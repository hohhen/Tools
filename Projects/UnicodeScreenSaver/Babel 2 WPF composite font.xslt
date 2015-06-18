﻿<?xml version="1.0" encoding="utf-8"?>
<!--Transform Composite Font mapping as generated by Babel Map to Composite font used by WPF-->
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:unicode="http://www.unicode.org/ns/2003/ucd/1.0"
                xmlns:System="clr-namespace:System;assembly=mscorlib"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                exclude-result-prefixes="msxsl" xmlns=""
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="/CompositeFont">
        <FontFamily
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/composite-font"
            Baseline="0.9" LineSpacing="1.2"
        >
            <FontFamily.FamilyNames>
                <System:String x:Key="en-US">Unicode composite font</System:String>
            </FontFamily.FamilyNames>

            <FontFamily.FamilyTypefaces>
                <FamilyTypeface Weight="Normal" Stretch="Normal" Style="Normal" UnderlinePosition="-0.1" UnderlineThickness="0.05" StrikethroughPosition="0.3"  StrikethroughThickness="0.05" CapsHeight="0.5" XHeight="0.3"/>
                <FamilyTypeface Weight="Bold" Stretch="Normal" Style="Normal" UnderlinePosition="-0.1" UnderlineThickness="0.05" StrikethroughPosition="0.3"  StrikethroughThickness="0.05" CapsHeight="0.5"         XHeight="0.3"/>
            </FontFamily.FamilyTypefaces>

            <FontFamily.FamilyMaps>
                <xsl:apply-templates/>
            </FontFamily.FamilyMaps>
        </FontFamily>
    </xsl:template>

    <xsl:template match="Block[text()]">
        <xsl:variable name="block" select="@name"/>
        <FontFamilyMap xmlns="http://schemas.microsoft.com/winfx/2006/xaml/composite-font"
                       Unicode="{document('UCD blocks.xml')/unicode:ucd/unicode:blocks/unicode:block[@name=$block]/@first-cp}-{document('UCD blocks.xml')/unicode:ucd/unicode:blocks/unicode:block[@name=$block]/@last-cp}"
                       Target="{text()}" Scale="1.0"
        >
            <xsl:comment>
                <xsl:value-of select="@name"/>
            </xsl:comment>
        </FontFamilyMap>
    </xsl:template>
</xsl:stylesheet>
