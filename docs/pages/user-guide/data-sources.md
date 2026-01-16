---
title: Data Sources
layout: libdoc_page.liquid
permalink: data-sources/index.html
eleventyNavigation:
    key: Data Sources
    parent: User Guide
    order: 3
date: 2026-01-16
---

You can link external data sources to your project, which allows you to create multiple variants of components by using merge fields to automatically customise individual components.

View/manage data sources by selecting the 'Data Sources' tab from the project page.

# Adding Data Sources

## Google Sheets

To link a Google Sheets data source to your project, click on the 'Data Sources' tab on the project overview screen, and click 'Add Data Source'. 

_In Google Sheets:_
 - Click on the Share button
 - Ensure the sheet is accessible by setting 'General Access' to 'Anyone with the link'
 - Click 'Copy Link' to get the Google Sheets URL

_In Deckle:_
 - Fill in the Google Sheets URL
 - Optional: Give the data source a name, to make it easy to find/reference later
 - If your data source has multiple sheets, by default Deckle will read the first; however if you want to add a different sheet, you will need to set the sheets `gid`. To get this, view the sheet in Google Sheets and check the browser url. You will see the gid in the url as `#gid=123456` or `?gid=123456`
 
Click 'Add Data Source' to finish adding it to Deckle.

You can verify that the data was linked correctly by clicking the 'View' button beside the data source in the list; this will show a preview of the data.

# Syncing Data Sources

Since data sources are external, Deckle doesn't automatically know when they are changed.

To sync the latest changes from a data source, simply click the 'Sync' button on the Data Sources tab. You can also sync data sources directly from the Data Sources panel at the bottom of the [Component Editor](/user-guide/component-editor)

# Removing Data Sources

Removing a data source 