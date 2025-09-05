# Docusign Instructions

## Important
Please ensure that the following configurations are updated in your application settings:

1. **private.key**: This file must be changed as per your requirements.

2. **App Settings**: Update the following keys:
   - **IntegrationKey**: Your integration key here.
   - **UserId**: Your user ID here.
   - **AuthServer**: Your authentication server URL here.
   - **BasePath**: Your base path here.
   - **AccountId**: Your account ID here.
   - **PrivateKeyFile**: The file path for your private key here.

Make sure to save and commit your changes after updating these configurations!
use this command to create database after changing you connection string
Update-Database -Project JobModule.Repository -StartupProject JobModule.Api
