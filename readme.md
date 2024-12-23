## Overview
A simple email notification service that uses SendGrid and MJML to send email notifications.

## Getting started
This repo provides a drop in service which can be used to send email notifications via either SendGrid or basic SMTP.

In order to use this repo in your project, clone the repo and in your `Startup.cs` file, call the `AddEmailNotifications()` method defined in the `DependencyInjection.cs` file.

Once added, you can inject the `EmailService` anywhere you need and send an email by calling the `SendEmailAsync()` method.

## Templates
Templates are defined in two places, the actual template content that is displayed in the email is defined in the `EmailTemplates` directory as a `.mjml` and `.txt` file. A sample has been provided to indicate what is needed in each.

Once the template itself is defined, a code representation of the template is needed. This is defined in the `Models` directory and _must_ implement the `ITemplate` interface. The template class is used to hold all the parameters that are required in the template that was defined in the `EmailTemplates` directory.

Finally, notification types are defined in the `NotificationType.cs` enum. This is used in the `EmailService.SendEmailAsync()` method to determine what email template to use based on the notification type.

## Settings
The repo requires certain settings to be set in order for the dependency injection to work. A sample of the settings required has been provided in the `appsettings.json` file. As this project is intended to be used as part of a larger solution, feel free to remove the `appsettings.json` file and place the contents in the appropriate place for your use case.

## Code formatting
This repo uses [CSharpier](https://csharpier.com/) for code formatting. Please ensure you have the relevant extension installed in your IDE before committing any work to this repo.
