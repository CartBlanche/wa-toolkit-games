#------------------------------------------------------------
# Evangelism Content Framework. PowerShell implementation
#------------------------------------------------------------

# Arguments

$env:ECF = [Environment]::GetEnvironmentVariable("ECF","Machine")

[string] $sampleManifestFile = [System.IO.Path]::GetFullPath($args[0])
[string] $outputDirectory = [System.IO.Path]::GetFullPath($args[1])

[bool] $createSeftExtracting = $false
[string] $arg2 = $args[2]
if ([System.String]::Compare($arg2, "SelfExtracting" , $false) -eq 0)
{	
	$createSeftExtracting = $true
}

[string] $assetsDirectory = ""

[string] $templatesDirectory = ""
if ($args[3] -ne "")
{	
    [string] $templatesDirectory = [System.IO.Path]::GetFullPath($args[3])	
}

# Directories
[string] $ecfResourcesDirectory = "$env:ECF\resources"
[string] $resourcesDirectory = [System.IO.Path]::GetFullPath(".\Resources")
[string] $sampleDirectory =[System.IO.Path]::GetDirectoryName($sampleManifestFile)

#temps
[string] $tempDirectory = [System.IO.Path]::Combine($outputDirectory, "temp")

$reader = [xml] (get-content $sampleManifestFile)
[string] $sampleId = $reader.Sample.Id;

[string] $relativeSample = [System.IO.Path]::Combine($sampleId, [System.IO.Path]::GetFileName($sampleManifestFile))

[string] $tempSampleFile = [System.IO.Path]::Combine($tempDirectory, $relativeSample) 
[string] $tempPackageDirectory = [System.IO.Path]::Combine($tempDirectory, $sampleId) 

#Remove the Log.xml file if it exists
[string] $logFile = [System.IO.Path]::Combine($packageDirectory, "Log.xml") 
if(test-path $logFile)
{
    Remove-item $logFile
}

#This script registers the SnapIn
add-pssnapin ContentFrameworkSnapIn

$ErrorActionPreference = "Stop"

#---------------------------------------------------------------------------
#-| CopySample Step -------------------------------------------------------
#---------------------------------------------------------------------------

[string] $beginExcludes = ".git obj bin csx build"

Copy-Sample $sampleManifestFile $tempDirectory $beginExcludes $assetsDirectory

write "Copy sample done."
#---------------------------------------------------------------------------

#---------------------------------------------------------------------------
#-| RemoveSoucreCodeBindings Step ------------------------------------------
#---------------------------------------------------------------------------

Remove-SourceControlBindings $tempDirectory

write "Source Control Bindings Removal done."
#---------------------------------------------------------------------------

#---------------------------------------------------------------------------
#-| AddHeaders Step --------------------------------------------------------
#---------------------------------------------------------------------------

[string] $copyrightFile = [System.IO.Path]::Combine($resourcesDirectory, "Copyright.txt")

Add-Headers $copyrightFile $tempDirectory

write "Adding headers done."
#---------------------------------------------------------------------------

#---------------------------------------------------------------------------
#-| ReviewDocument -------------------------------------------------------
#---------------------------------------------------------------------------

Review-Document -Sample $tempSampleFile -Rules @("ContentFramework.Reviewer.Rules.TopicRules, ContentFramework.Reviewer")

write "Review documents done."
#---------------------------------------------------------------------------

#---------------------------------------------------------------------------
#-| Create Pages Step ------------------------------------------------------
#---------------------------------------------------------------------------

if (("$templatesDirectory" -ne "") -AND (test-path "$templatesDirectory"))
{
	[string] $xsltPath = [System.IO.Path]::Combine($resourcesDirectory, "HtmlConversion\xsl")
	[string] $includeTemplates = "*.include.t4 *.include.tt"

	Create-SampleNavigationSite $tempSampleFile $templatesDirectory $xsltPath $tempDirectory $includeTemplates

	# Removed unused htm page
	[string] $sampleHtmlPath = [System.IO.Path]::Combine($tempPackageDirectory, "Sample.htm")
	if (test-path $sampleHtmlPath)
	{
		Remove-Item $sampleHtmlPath
	}

	# move navigation Site to the "docs" folder
	Move-Item "$tempPackageDirectory\*.htm" "$tempPackageDirectory\docs"
	Move-Item "$tempPackageDirectory\images" "$tempPackageDirectory\docs"
	Move-Item "$tempPackageDirectory\styles" "$tempPackageDirectory\docs"
	Move-Item "$tempPackageDirectory\scripts" "$tempPackageDirectory\docs"

	if (Test-Path "$tempPackageDirectory\docs\Readme.html")
	{
		Remove-Item "$tempPackageDirectory\docs\Readme.html\html\*.html" -force -recurse
	}

	# Add StartHere.htm
	[string] $startPage = [System.IO.Path]::Combine($tempPackageDirectory, "docs\Overview.htm")
	if (Test-Path "$startPage")
	{
		set-content -path "$tempPackageDirectory\StartHere.htm" -value "<html><head><meta http-equiv='REFRESH' content='0; url=./docs/Overview.htm'></head><body></body></html>"
	}
	

	write "Creating navigation pages done."
}
#---------------------------------------------------------------------------

#---------------------------------------------------------------------------
#-| Create Zip or Self Extracting ------------------------------------------
#---------------------------------------------------------------------------
$trimmedSampleId = $sampleId
if ($trimmedSampleId.ToUpper().StartsWith("WINDOWSAZURE\")) 
{
	$trimmedSampleId = $trimmedSampleId.Substring(13)
}

if($createSeftExtracting)
{
	write "Creating self extracting ..."

	[string] $name = $trimmedSampleId + ".Setup.exe"
	[string] $licenseFile = [System.IO.Path]::Combine($resourcesDirectory, "LicenseAgreement.txt")
	[string] $imageFile = [System.IO.Path]::Combine($resourcesDirectory, "vertical.bmp")
	[string] $excludeFile = [System.IO.Path]::Combine($resourcesDirectory, "Exclude.txt")

	Create-SelfExtract $name $tempSampleFile $outputDirectory $licenseFile $imageFile $excludeFile
}
else
{
	write "Creating Zip file ..."
	[string] $name = $trimmedSampleId + ".zip"
	[string] $zipName = [System.IO.Path]::Combine($outputDirectory, $name)
	[string] $excludeFile = [System.IO.Path]::Combine($resourcesDirectory, "Exclude.txt")

	Create-Zip $tempDirectory $zipName $excludeFile
}

write "Packaging Sample Finished."
#---------------------------------------------------------------------------