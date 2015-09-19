<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes" encoding="utf-8"/>

  <xsl:template match="/Menus">
    <ul>
      <xsl:attribute name="class">
        <xsl:text>sf-menu</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="id">
        <xsl:text>nav</xsl:text>
      </xsl:attribute>
      <xsl:call-template name="MenuListing" />
    </ul>
  </xsl:template>
  <!-- Allow for recusive child node processing -->
  <xsl:template name="MenuListing">
    <xsl:apply-templates select="aspnet_Tabs" />
  </xsl:template>

  <xsl:template match="aspnet_Tabs">
    <!--   Convert Menu child elements to <li> <a> html tags and  add attributes inside <a> tag -->
    <li>              
      <a>
        <xsl:attribute name="href">
          <xsl:value-of select="concat('/', TabPath)"/>
        </xsl:attribute>
        <xsl:value-of select="Title"/>
      </a>
      <!-- Call MenuListing if there are child Menu nodes -->
      <xsl:if test="count(aspnet_Tabs) > 0">
        <ul>
          <xsl:call-template name="MenuListing" />
        </ul>
      </xsl:if>
    </li>
  </xsl:template>
</xsl:stylesheet>
