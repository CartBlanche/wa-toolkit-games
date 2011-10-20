function UpdateConfigurationSetting($configurationFile, $settingValue, $settingKey)
{
    [xml]$xml = get-content $configurationFile;
	
	$xml.ServiceConfiguration.Role | 
	  ForEach-Object  { $_.ConfigurationSettings.Setting } | 
	  Where-Object { $_.name -match $settingKey } | 
	  ForEach-Object  { $_.value = $settingValue;}
	  
    $xml.Save($configurationFile);
}

function UpdateWebConfigurationSetting($configurationFile, $settingValue, $settingKey)
{
    [xml]$xml = get-content $configurationFile;
	$entry = $xml.configuration.appSettings.add | Where-Object { $_.key -match $settingKey }
	$entry.value = $settingValue;

    $xml.Save($configurationFile);
}

function UpdateWebConfigurationConnectionSetting($configurationFile, $connectionString, $settingKey)
{
    [xml]$xml = get-content $configurationFile;
	$entry = $xml.configuration.connectionStrings.add | Where-Object { $_.name -match $settingKey }
	$entry.connectionString = $connectionString;

    $xml.Save($configurationFile);
}

# ------------------------------ 
# Obtaining Configuration Values
# ------------------------------
$configurationFilePath = "..\..\..\Configuration.xml";
[xml]$xml = Get-Content $configurationFilePath;
$serverName = $xml.Configuration.Database.ServerName;
$databaseName = $xml.Configuration.Database.DatabaseName;
$username = $xml.Configuration.Database.Username;
$password = $xml.Configuration.Database.Password;
$storageAccountName = $xml.Configuration.WindowsAzureStorage.AccountName.ToLower();
$storageAccountKey = $xml.Configuration.WindowsAzureStorage.AccountKey;

$sqlServerScriptPath = "..\database\SqlServer.Setup.cmd";
$sqlAzureScriptPath = "..\database\SqlAzure.Setup.cmd";
$localServiceConfigurationPath = "..\..\..\code\SocialGames.Cloud\ServiceConfiguration.Local.cscfg";
$webConfigurationPath = "..\..\..\code\SocialGames.Web\Web.config";

# ------------------
# Creating database 
# ------------------

Write-Output ""
Write-Output "Creating and populating the database..."

if($serverName.Contains("database.windows.net"))
{
    & $sqlAzureScriptPath $serverName $databaseName $username $password;
}
else 
{
    & $sqlServerScriptPath $serverName $databaseName;
}

# -----------------------------
# Updating connection strings 
# -----------------------------

Write-Output ""
Write-Output "Updating connection strings..."

if($serverName.Contains("database.windows.net")) 
{
    $connectionString = "Server=tcp:$serverName;Database=$databaseName;User ID=$username;Password=$password";
}
else
{
    $connectionString = "Data Source=$serverName;Initial Catalog=$databaseName;Integrated Security=True";    
}

$settingKey = "StatisticsConnectionString";
UpdateConfigurationSetting $localServiceConfigurationPath $connectionString $settingKey;
UpdateWebConfigurationConnectionSetting $webConfigurationPath $connectionString $settingKey;

if(!$storageAccountName -or !$storageAccountKey) 
{
    $connectionString = "UseDevelopmentStorage=true";
	$blobEndpoint = "http://127.0.0.1:10000/devstoreaccount1/";
}
else
{
    $connectionString = "DefaultEndpointsProtocol=https;AccountName=$storageAccountName;AccountKey=$storageAccountKey";
	$blobEndpoint = "http://$storageAccountName.blob.core.windows.net/";
}

$settingKey = "DataConnectionString";
UpdateConfigurationSetting $localServiceConfigurationPath $connectionString $settingKey;
UpdateWebConfigurationSetting $webConfigurationPath $connectionString $settingKey;

$settingKey = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
UpdateConfigurationSetting $localServiceConfigurationPath $connectionString $settingKey;

$settingKey = "BlobUrl";
UpdateWebConfigurationSetting $webConfigurationPath $blobEndpoint $settingKey;
