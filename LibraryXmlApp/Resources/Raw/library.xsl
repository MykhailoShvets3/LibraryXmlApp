<?xml version="1.0" encoding="utf-8"?>
<!-- XSL-таблиця стилів: перетворює library.xml у HTML-таблицю книг із читачами. -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" encoding="utf-8" indent="yes"/>

	<xsl:template match="/library">
		<html>
			<head>
				<title>Кафедральна бібліотека</title>
				<style>
					table { border-collapse: collapse; }
					th, td { border: 1px solid #888; padding: 4px 8px; vertical-align: top; }
				</style>
			</head>
			<body>
				<h2>Кафедральна бібліотека</h2>
				<table>
					<tr>
						<th>ID</th>
						<th>Назва</th>
						<th>Автор</th>
						<th>Факультет</th>
						<th>Кафедра</th>
						<th>Рік</th>
						<th>Анотація</th>
						<th>Видавець</th>
						<th>Читачі</th>
					</tr>
					<xsl:apply-templates select="book"/>
				</table>
			</body>
		</html>
	</xsl:template>

	<!-- Атрибути, яких у книги немає, дають порожню клітинку -->
	<xsl:template match="book">
		<tr>
			<td>
				<xsl:value-of select="@id"/>
			</td>
			<td>
				<b>
					<xsl:value-of select="@title"/>
				</b>
			</td>
			<td>
				<xsl:value-of select="@author"/>
			</td>
			<td>
				<xsl:value-of select="@faculty"/>
			</td>
			<td>
				<xsl:value-of select="@department"/>
			</td>
			<td>
				<xsl:value-of select="@year"/>
			</td>
			<td>
				<xsl:value-of select="@annotation"/>
			</td>
			<td>
				<xsl:value-of select="@publisher"/>
			</td>
			<td>
				<xsl:for-each select="reader">
					<p>
						<xsl:value-of select="@name"/> (<xsl:value-of select="@position"/>, <xsl:value-of select="@department"/>)
					</p>
				</xsl:for-each>
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>