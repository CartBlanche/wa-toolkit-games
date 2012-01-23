function GetConfigurationValue($entry)
{
	if(-not ($entry)) 
	{ 
		return ""; 
	}
	
	if($entry.StartsWith('{'))
	{
		return "";
	}
	return $entry;
}

if ((Get-PSSnapin | ?{$_.Name -eq "WAPPSCmdlets"}) -eq $null) {
	Add-PSSnapin WAPPSCmdlets	
} 

$scriptDir = (Split-Path $myinvocation.mycommand.path -parent)
Set-Location $scriptDir


[xml] $xml = get-Content "..\..\..\configuration.xml"
[string] $subscriptionId = GetConfigurationValue($xml.Configuration.subscriptionId)
[string] $hostedServiceName = GetConfigurationValue($xml.Configuration.hostedService.name)
[string] $storageAccountName = GetConfigurationValue($xml.Configuration.WindowsAzureStorage.StorageAccountName)
[string] $packageFile = GetConfigurationValue($xml.Configuration.deployment.packageFile)
[string] $configurationFile = GetConfigurationValue($xml.Configuration.deployment.configurationFile)
[string] $deploymentLabel = GetConfigurationValue($xml.Configuration.deployment.label)
[string] $thumbprint = GetConfigurationValue($xml.Configuration.managementCertificateThumbprint).ToUpper()

if (-not ([System.IO.Path]::IsPathRooted("$packageFile")))
{
	$packageFile = Join-Path "$scriptDir" "$packageFile"
}	
if (-not ([System.IO.Path]::IsPathRooted("$configurationFile")))
{
	$configurationFile = Join-Path "$scriptDir" "$configurationFile"
}

# Get the management API certificate 
if ((Test-Path cert:\CurrentUser\MY\$thumbprint) -eq $true){
	$certificate = (Get-Item cert:\CurrentUser\MY\$thumbprint)
} else {
	$certificate = (Get-Item cert:\LocalMachine\MY\$thumbprint)
}

# Get the hosted service 
$hostedService = Get-HostedService -ServiceName $hostedServiceName -SubscriptionId $subscriptionId -Certificate $certificate 

#---------------------------------------------

# If there's a deployment on staging we will wipe it out
if(($hostedService | Get-Deployment Staging).DeploymentId -ne $null) {
    Write-Host "Wiping Existing Staging Deployment"
	
	# Set deployment to suspended
	$hostedService | Get-Deployment -Slot Staging | Set-DeploymentStatus Suspended | Get-OperationStatus -WaitToComplete

	# Remove the Staging Deployment as it's where we are going to publish (mandatory for deletion)
	$hostedService | Get-Deployment -Slot Staging | Remove-Deployment | Get-OperationStatus -WaitToComplete
}

Write-Host "Deploying Latest Package"
# Deploy latest package version

if ($deploymentLabel -eq "")
{
	$deploymentLabel = "Social Games"
}

$hostedService | New-Deployment -Slot Staging -StorageServiceName $storageAccountName -Package $packageFile -Configuration $configurationFile -Label $deploymentLabel | Get-OperationStatus -WaitToComplete

Write-Host "Starting Deployed Service"
# Start the just deployed service
$hostedService | Get-Deployment -Slot Staging | Set-DeploymentStatus -Status Running | Get-OperationStatus -WaitToComplete

Write-Host "Performing VIP Swap" 
# Promote to production 
$hostedService | Get-Deployment -Slot Staging | Move-Deployment | Get-OperationStatus -WaitToComplete