param([string]$webConfigPath)

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

function UpdateWebConfigurationSetting($configurationFile, $settingValue, $settingKey)
{
	[xml]$xml = get-content $configurationFile;
	$entry = $xml.configuration.appSettings.add | Where-Object { $_.key -match $settingKey }
	$entry.value = $settingValue;

    $xml.Save($configurationFile);
}

if ((Get-PSSnapin | ?{$_.Name -eq "WAPPSCmdlets"}) -eq $null) {
	Add-PSSnapin WAPPSCmdlets	
} 

$scriptDir = (Split-Path $myinvocation.mycommand.path -parent)
Set-Location $scriptDir


[xml] $xml = get-Content "..\..\..\configuration.xml"
[string] $subscriptionId = GetConfigurationValue($xml.Configuration.subscriptionId)
[string] $hostedServiceName = GetConfigurationValue($xml.Configuration.hostedService.name)
[string] $hostedServiceLabel = GetConfigurationValue($xml.Configuration.hostedService.label)
[string] $hostedServiceLocation = GetConfigurationValue($xml.Configuration.hostedService.location)
[string] $storageAccountName = GetConfigurationValue($xml.Configuration.WindowsAzureStorage.StorageAccountName)
[string] $storageAccountLabel = GetConfigurationValue($xml.Configuration.WindowsAzureStorage.StorageAccountLabel)
[string] $storageAccountLocation = GetConfigurationValue($xml.Configuration.WindowsAzureStorage.StorageAccountLocation)
[string] $packageFile = GetConfigurationValue($xml.Configuration.deployment.packageFile)
[string] $configurationFile = GetConfigurationValue($xml.Configuration.deployment.configurationFile)
[string] $deploymentLabel = GetConfigurationValue($xml.Configuration.deployment.label)
[string] $thumbprint = GetConfigurationValue($xml.Configuration.managementCertificateThumbprint).ToUpper()
[string] $rdpCertificatePath = GetConfigurationValue($xml.Configuration.rdpCertificatePath)
[string] $rdpCertificatePassword = GetConfigurationValue($xml.Configuration.rdpCertificatePassword)
[string] $sslCertificatePath = GetConfigurationValue($xml.Configuration.sslCertificatePath)
[string] $sslCertificatePassword = GetConfigurationValue($xml.Configuration.sslCertificatePassword)

$webConfigurationPath = "$webConfigPath\Web.config";

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
$hostedService = Get-HostedServices -SubscriptionId $subscriptionId -Certificate $certificate | where {$_.ServiceName -eq $hostedServiceName}
# If there's a no hosted service with that name then we will create it
if ($hostedService -eq "")
{
	Write-Host "Creating new Hosted Service"
    # Create and Retrieve the hosted service
	New-HostedService -ServiceName $hostedServiceName -Label $hostedServiceLabel -Location $hostedServiceLocation -SubscriptionId $subscriptionId -Certificate $certificate | Get-OperationStatus -WaitToComplete
    $hostedService = Get-HostedService -ServiceName $hostedServiceName -SubscriptionId $subscriptionId -Certificate $certificate
}

Write-Host "Adding Certificates to HostedService"
# Add the certificate to the hosted service (doesn't matter it is there already)
if ($rdpCertificatePath -ne "")
{
	if (-not ([System.IO.Path]::IsPathRooted("$rdpCertificatePath")))
	{
		$rdpCertificatePath = Join-Path "$scriptDir" "$rdpCertificatePath"
	}
	
	($hostedService | Add-Certificate -CertificateToDeploy $rdpCertificatePath -Password $rdpCertificatePassword) | out-null
}
if ($sslCertificatePath -ne "")
{
	if (-not ([System.IO.Path]::IsPathRooted("$sslCertificatePath")))
	{
		$sslCertificatePath = Join-Path "$scriptDir" "$sslCertificatePath"
	}
	
	($hostedService | Add-Certificate -CertificateToDeploy $sslCertificatePath -Password $sslCertificatePassword) | out-null
}

# Get the storage account 
$storageAccount = Get-StorageAccount -SubscriptionId $subscriptionId -Certificate $certificate | where {$_.ServiceName -eq $storageAccountName}
# If there's a nostorage account with that name then we will create it

if ($storageAccountLabel -eq "")
{
	$storageAccountLabel = "Social Games"
}

if ($storageAccount -eq "")
{
	Write-Host "Creating new Storage Account"
    # Create the storage account
	New-StorageAccount -ServiceName $storageAccountName -Label $storageAccountLabel -Location $storageAccountLocation -SubscriptionId $subscriptionId -Certificate $certificate | Get-OperationStatus -WaitToComplete    
}

# Retrieve the storage account key
$storageAccountKey = (Get-StorageKeys -ServiceName $storageAccountName -SubscriptionId $subscriptionId -Certificate $certificate ).Primary

# Update DataConnectionString in Package configuration file
$connectionString = "DefaultEndpointsProtocol={0};AccountName={1};AccountKey={2}" -f "https", $storageAccountName, $storageAccountKey

$settingKey = "DataConnectionString";
UpdateWebConfigurationSetting $webConfigurationPath $connectionString $settingKey;

$settingKey = "ApiUrl";
$apiEndpoint = "http://$hostedServiceName.cloudapp.net/";
UpdateWebConfigurationSetting $webConfigurationPath $apiEndpoint $settingKey;

$settingKey = "BlobUrl";
$blobEndpoint = "http://$storageAccountName.blob.core.windows.net/";
UpdateWebConfigurationSetting $webConfigurationPath $blobEndpoint $settingKey;

