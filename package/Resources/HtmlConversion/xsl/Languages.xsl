<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:template name="LanguageDropdown">
		<div id="languageSpan">
			<xsl:for-each select="$config/languages/language">
				<input type="checkbox" name="languageFilter" onclick="SetLanguage(this)" id="{@checkboxId}" />
				<label class="languageFilter" for="{@checkboxId}"><xsl:text/><xsl:value-of select="."/><xsl:text/></label><br/>
			</xsl:for-each>
		</div>
	</xsl:template>
	
</xsl:stylesheet>
