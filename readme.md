# Scoped Authentication

This sample application builds on the  [WebApi Owin Authorization ](https://auth0.com/docs/quickstart/backend/webapi-owin/01-authorization) and [AngularJS: Login](https://auth0.com/docs/quickstart/spa/angularjs/01-login) tutorials to demonstrate scoped authorization.  In this example the endpoints a user is authorized to call is restricted by their job title.  Depending on that job title a user will be able to access either a read, write, or delete endpoint.  

## Summary
At a high level the flow during this sample is as follows:

1. User logs in.
2. Auth0 returns an access_token that includes a scope.
3. Client treats the user as a logged in user and can access the scope in the token for client side validation.
4. Responding to user input the client calls a service endpoint and includes the access_token in the Authorization header.
5. In addition to checking the token to authenticate the user, the service checks the scope on the token to see whether the user is authorized to use the particular endpoint being called.  


## Dashboard Setup

In the Auth0 dashboard we will define three scopes indicating what operations the user is able to perform.  We will then configure a rule that includes this scope in the access_token so when it is passed to the Api this scope can be checked to determine whether the user has access to the endpoint. 

1. The first step is to create our application.  
    1. In the Auth0 Dashboard click the Add Application button.
    2. Give your application a name and for this sample chose Single Page Application as the application type. Click create.
    3. Go the settings tab for the new application.  Save the domain and client id as they will be needed later.
	4. Scroll down until you see the Allowed Callback URLs dialog and enter http://localhost:3000/callback/callback.html.  This url is the address Auth0 will return the access token to upon user login.  
2. Next we create the api.  
	1. In the Auth0 Dashboard go to the api tab and click the Create Api button.
	2. Provide a name and identifier for the api, leave the signing algorithm to the default value, and click create.
	3. Go to the settings tab for the new api.  Save the identifier as it will be needed later.
	4. Go to the Scopes tab for the new api and add these scopes: read:items, write:items, and delete:items.  These will indicate what operation a user is able to perform on a mock items endpoint.  
3. Next we will create three users and assign each a job title.  
	1. In the Auth0 Dashboard go to the users tab.  If you do not have three users create them. Naming them a variation of reader, writer, and deleter would be helpful.  
	2. Go to User Details for the reader user and scroll down to app_metadata.  Enter the following property in the app_metadata json "jobTitle": "ItemReader".  
	3. Go to User Details for the writer user and scroll down to app_metadata.  Enter the following property in the app_metadata json "jobTitle": "ItemWriter".  
	4. Go to User Details for the deleter user and scroll down to app_metadata.  Enter the following property in the app_metadata json "jobTitle": "ItemDeleter".  

4.  Last we will create a rule to set the users scope in the access token returned upon a successful login.  
	1. In the Auth0 Dashboard go to the rules tab. 
	2. Click empty rule.
	3. Name your rule and insert this javascript into the below rule.  The script reads the job title we created in step 3 from the app_metadata property of the user parameter.  Based on the title it then sets the scope property of the access token being returned. 
	
```javascript 
function (user, context, callback) {
   let scope;
   switch(user.app_metadata.jobTitle){
      case "ItemReader":
        scope = 'read:items';
        break;
      case "ItemWriter":
        scope = 'write:items';
        break;
      case "ItemDeleter":
        scope = 'delete:items';
        break;
      default:
        break;
    }
  
  context.accessToken.scope = [scope];
  callback(null, user, context);
} 
```


## AngularJS Setup

The application in this sample is an AngularJS application setup similarly to the 
[AngularJS: Login](https://auth0.com/docs/quickstart/spa/angularjs/01-login) tutorial.  
In the app.js file  angularAuth0Provider.init should be configured using the clientId and domain from the application configured in the dashboard, while redirectUri should match the url we set  audience should be the identifier configured for the api in the dashboard.  

```javascript
 angularAuth0Provider.init({
      clientID: 'FLC8nTogJJ25M5vIUFiTn76aQHli7bN2',
      domain: 'mattbtest.auth0.com',
      responseType: 'token id_token',
      audience: 'https://auth0sampleapi',
      redirectUri: 'http://localhost:3000/callback/callback.html',
      scope: 'openid'
    });
```

This example adds an app.constants.es6 file that includes a constant defining the url to our api service.  If the default needs to be changed it can be done here.  


## Running the example

In order to run the example you need to just start a server. What we suggest is doing the following:

1. Make sure `web.config` contains your credentials. You can find your credentials in the settings section of your Auth0 Client.
2. Hit F5 to start local web development server.

Go to `http://localhost:58105/api/ping` and you'll see the app running :).


## Running the example

In order to run the example you need to just start a server. What we suggest is doing the following:

1. Make sure `web.config` contains your credentials. You can find your credentials in the settings section of your Auth0 Client.
2. Hit F5 to start local web development server.

Go to `http://localhost:58105/api/ping` and you'll see the app running :).