param([string]$configurationFilePath, [string]$serviceConfigurationPath, [string]$WebConfigPath)

function UpdateWebConfig($configurationFile, $issuer, $relyingPartyRealm, $signingKey)
{
    [xml]$xml = get-content $configurationFile;

	$identityModel = $xml.configuration."microsoft.identityModel"
	$audienceUri = $identityModel.service.audienceUris.add | where { $_.value -eq $relyingPartyRealm }
    
    if ($audienceUri -eq $null)
    {
        $audienceUri = $xml.CreateElement("add")
        $audienceUri.SetAttribute("value", $relyingPartyRealm)
        $xml.configuration."microsoft.identityModel".service.audienceUris.AppendChild($audienceUri)
    }
    
    $identityModel.service.issuerNameRegistry.trustedIssuers.add.thumbprint = $signingKey
	$identityModel.service.issuerNameRegistry.trustedIssuers.add.name = $issuer
    
	$xml.Save($configurationFile);
}

# ------------------------------ 
# Obtaining Configuration Values
# ------------------------------
$configurationFilePath = "$configurationFilePath\Configuration.xml";
[xml]$xml = Get-Content $configurationFilePath;    

$acsNamespace = $xml.Configuration.AccessControlService.Namespace;
$mgmtKey = $xml.Configuration.AccessControlService.ManagementKey;
$relyingPartyRealm = $xml.Configuration.AccessControlService.RelyingPartyRealm;

$useYahooIdentityProvider = [System.Convert]::ToBoolean($xml.Configuration.AccessControlService.UseYahooIdentityProvider.ToLower());
$useGoogleIdentityProvider = [System.Convert]::ToBoolean($xml.Configuration.AccessControlService.UseGoogleIdentityProvider.ToLower());
$useWindowsLiveIdentityProvider = [System.Convert]::ToBoolean($xml.Configuration.AccessControlService.UseWindowsLiveIdentityProvider.ToLower());
$useFacebookIdentityProvider = [System.Convert]::ToBoolean($xml.Configuration.AccessControlService.UseFacebookIdentityProvider.ToLower());

$useLocalComputeEmulator = [System.Convert]::ToBoolean($xml.Configuration.UseLocalComputeEmulator.ToLower());

if ($useFacebookIdentityProvider)
{
    $facebookApplicationName = $xml.Configuration.AccessControlService.FacebookApplicationName;
	$facebookApplicationId = $xml.Configuration.AccessControlService.FacebookApplicationId;
    $facebookSecret = $xml.Configuration.AccessControlService.FacebookSecret;
}

$allowedIdentityProviders = @();

# -----------------
# Configuring ACS
# -----------------

Write-Output ""
Write-Output "Configuring Access Control Service..."

# Include Windows Azure Cmdlets SnapIn
Add-PSSnapin "WAPPSCmdlets";

$mgmtToken = Get-AcsManagementToken -namespace $acsNamespace -managementKey $mgmtKey;

# Preconfigured Identity Providers
if ($useWindowsLiveIdentityProvider)
{
    Write-Output "Configuring Windows Live ID as Identity Provider...";
	$allowedIdentityProviders += "Windows Live ID";
}

if ($useYahooIdentityProvider) 
{
    Write-Output "Configuring Yahoo! as Identity Provider...";
    $dummy = Add-IdentityProvider -mgmtToken $mgmtToken -type "Preconfigured" –preconfiguredIPType "Yahoo!";
	$allowedIdentityProviders += "Yahoo!";
}

if ($useGoogleIdentityProvider)
{
    Write-Output "Configuring Google as Identity Provider...";
	$dummy = Add-IdentityProvider -mgmtToken $mgmtToken -type "Preconfigured" –preconfiguredIPType "Google";
	$allowedIdentityProviders += "Google";
}

if ($useFacebookIdentityProvider)
{
    Write-Output "Configuring Facebook as Identity Provider...";

    # Remove FB App IP (if exists)
    Remove-IdentityProvider -mgmtToken $mgmtToken -name $facebookApplicationName;
    
    # Add FB App IP
    $dummy = Add-IdentityProvider -mgmtToken $mgmtToken -type "FacebookApp" -name $facebookApplicationName -fbAppId $facebookApplicationId -fbAppSecret $facebookSecret;
	$allowedIdentityProviders += $facebookApplicationName;
}

# Configure Relying Party

if($useLocalComputeEmulator)
{
	$rpName = "Social Games Local";
	$groupName = "Default Rule Group for $rpName";
	$rpReturnUrl = "https://127.0.0.1/Account/LogOn";
}
else
{
	$rpName = "Social Games Azure";
	$groupName = "Default Rule Group for $rpName";
	$rpReturnUrl = $relyingPartyRealm + "Account/LogOn";
}

# Remove RP (if exists)
Write-Output "Remove Relying Party ($rpName) if exists...";
Remove-RelyingParty -mgmtToken $mgmtToken -name $rpName;

Write-Output "Remove All Rules In Group ($groupName) if exists...";
Get-Rule -mgmtToken $mgmtToken -groupName $groupName | ForEach-Object { Remove-Rule -mgmtToken $mgmtToken -rule $_ };

# Exit if Relying Party exists
$rps = Get-RelyingParty -Namespace $acsNamespace -ManagementKey $mgmtKey

foreach ($rp in $rps)
{
    if($rp.Realm.Equals($relyingPartyRealm))
    {
        Write-Host -fore red "Error: There is already a relying party with the realm: '" $relyingPartyRealm "'. Choose a different realm for the application and try again."
        exit 1;        
    }
}

# Create Relying Party
Write-Output "Create Relying Party ($rpName)...";
$rp = Add-RelyingParty -mgmtToken $mgmtToken -name $rpName -realm $relyingPartyRealm -returnUrl $rpReturnUrl -ruleGroup $groupName -allowedIdentityProviders $allowedIdentityProviders;

# Generate default rules
Write-Output "Create Default Passthrough Rules for the configured IPs...";
$rp.IdentityProviders | ForEach-Object { Add-DefaultPassthroughRules -mgmtToken $mgmtToken -groupName $groupName -identityProviderName $_.Name }

# -----------------------------
# Updating web.config
# -----------------------------

Write-Output ""
Write-Output "Updating web.config..."

$configurationFile = "$WebConfigPath\web.config";
$signingKey = Get-ServiceKey -mgmtToken $mgmtToken | where { $_.IsPrimary -and ($_.Type -eq "X509Certificate") -and ($_.Usage -eq "Signing") };
$issuer = "https://$acsNamespace.accesscontrol.windows.net/";

$encoder = new-Object System.Text.ASCIIEncoding
$psw = $encoder.GetString([System.Convert]::FromBase64String($signingKey.Password));

$cert = new-Object System.Security.Cryptography.X509Certificates.X509Certificate2($signingKey.Value, $psw);

UpdateWebConfig $configurationFile $issuer $relyingPartyRealm $cert.Thumbprint;


