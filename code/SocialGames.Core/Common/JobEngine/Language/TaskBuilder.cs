namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class TaskBuilder<T> : ITriggeredByBuilder<T>, IDoBuilder
    {
        private readonly Func<bool> condition;
        private readonly Func<bool, T> parametrizedConditionCheck;
        private readonly Action<T> parametrizedConditionConfirm;

        private Action<T, IDictionary<string, object>> setupContextCallback;
        private ICommand[] commands;
        private Action<ICommand, IDictionary<string, object>, Exception> errorCallback;
        private bool continueOnError;

        public TaskBuilder(Func<bool> condition)
        {
            this.condition = condition;
        }

        public TaskBuilder(CheckConditionWithAcceptCallback<T> condition)
        {
            this.parametrizedConditionCheck = condition.CheckFunc;
            this.parametrizedConditionConfirm = condition.ConfirmFunc;
        }

        public static bool AutomaticRetry { get; set; }

        public IDoBuilder SetupContext(Action<T, IDictionary<string, object>> call)
        {
            this.setupContextCallback = call;
            return this;
        }

        public IDoBuilder Do(params ICommand[] commandOrCommands)
        {
            this.commands = commandOrCommands;
            return this;
        }

        public IDoBuilder OnError(Action<ICommand, IDictionary<string, object>, Exception> call)
        {
            this.errorCallback = call;
            return this;
        }

        public IDoBuilder ContinueOnError()
        {
            this.continueOnError = true;
            return this;
        }

        public void Start(ExecutionModel execution)
        {
            ThreadPool.QueueUserWorkItem(s =>
                {
                    while (true)
                    {
                        Cycle();
                    }
                });
        }

        public void Start()
        {
            this.Start(ExecutionModel.AllWorkers);
        }

        protected void Cycle()
        {
            var context = new Dictionary<string, object>();

            if (this.condition != null && this.condition.Invoke())
            {
                this.ExecuteTasks(context);
            }
            else if (this.parametrizedConditionCheck != null)
            {
                T output;
                if (this.parametrizedConditionCheck.Invoke(out output))
                {
                    if (this.setupContextCallback != null)
                    {
                        this.setupContextCallback(output, context);
                    }

                    bool batchExecutedSuccessfully = this.ExecuteTasks(context);

                    // mark condition as processed even when some task throw an error
                    // if an error is generated, queue messages should be manually added
                    // For automatic retrying, set AutomaticRetry
                    if (!AutomaticRetry || batchExecutedSuccessfully)
                    {
                        this.parametrizedConditionConfirm.Invoke(output);
                    }
                }
            }

            // TODO: find out the best default value for this 
            // and provide a way to specify that in API
            this.Sleep(200);
        }

        protected virtual void Sleep(int interval)
        {
            Thread.Sleep(interval);
        }

        private bool ExecuteTasks(IDictionary<string, object> context)
        {
            var commandStack = new Stack<ICommand>();
            var errorDetected = false;

            foreach (var cmd in this.commands)
            {
                try
                {
                    commandStack.Push(cmd);
                    cmd.Do(context);
                }
                catch (Exception ex)
                {
                    errorDetected = true;

                    if (this.errorCallback != null)
                    {
                        this.errorCallback(commandStack.Peek(), context, ex);
                    }

                    if (!this.continueOnError)
                    {
                        break;
                    }
                }
            }

            return !errorDetected;
        }
    }
}