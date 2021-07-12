# AzureAD-Role-based-Group-Based-Access-control
Contains a web app and Api that are secured using azure ad. 
The api exposes roles such that only users with specific roles can access certain endpoints. 
Also uses group based auth. privacy page in the web app can only be accessed by users/application that belong to a certain azure ad group

Contains a authorization handler in the web project that checks if the user / application belongs to an azure ad group
