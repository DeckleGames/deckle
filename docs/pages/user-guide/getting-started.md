---
title: Getting Started
layout: libdoc_page.liquid
permalink: user-guide/getting-started/index.html
eleventyNavigation:
    key: Getting Started
    parent: User Guide
    order: 1
---

# Account Setup / Sign In

Simply use the 'Sign In with Google' button to log in using a Google account. If it is the first time using Deckle your account will be setup automatically, you don't need to do anything other than log in and start exploring!

# Creating your first project

When you first log in you will be taken to a home page, which will eventually show a list of all of your projects in Deckle.

To get started, simply click 'New Project' to add a project. All it needs for now is a name, and an optional description.

# Components

[Components](/user-guide/components) are the elements that make up your game. These can be cards, player mats, dice etc.

{% alert 'Reminder: Deckle is in early development, so you can only add a limited variety of component types currently, however more will be coming' 'info' %}

Some components - such as dice - are basic game elements. You can configure the number of these, properties such a colour, type (D6, D10, D20 etc).

Others - card, player mats, game boards etc - allow you to create custom designs using the Component Editor. If you link a data source to a component, you can easily create multiple variants of components by using merge fields to automatically customise individual components

To add a component, click the 'Add Component' button, select the type of component that you wish to create, and then enter the configuration options. The component will be created and added to the project. For designable/exportable components, you will see a list of actions (such as "Edit Front Design") which you can select to go to the Component Editor and start customising the design.

# Data Sources (Google Sheets)

To link a Google Sheets [data source](/user-guide/data-sources) to your project, click on the 'Data Sources' tab on the project overview screen, and click 'Add Data Source'. 

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

# Exporting

Deckle offers export functionality to allow you to print your game components for easy prototyping.

You can access the Export screen from the Components tab on the projects page.