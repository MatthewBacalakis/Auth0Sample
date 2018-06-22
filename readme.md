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
	4. Scroll down until you see the Allowed Callback URLs dialog and enter http://localhost:3000/callback . This url is the address Auth0 will return the access token to upon user login.  
2. Next we create the api.  
	1. In the Auth0 Dashboard go to the api tab and click the Create Api button.
	2. Provide a name and identifier for the api, leave the signing algorithm to the default value, and click create.
	3. Go to the settings tab for the new api.  Save the identifier as it will be needed later.
	4. In the settings tab enable Allow Offline Access which will allow a refresh token to be generated.  We are doing this for demo purposes for the UWP native app sample only.  Our SPA example follows the implicit flow which should not be used with refresh tokens in a production application as a SPA cannot secure a refresh token.  
	5. Go to the Scopes tab for the new api and add these scopes: read:items, write:items, and delete:items.  These will indicate what operation a user is able to perform on a mock items endpoint.  
3. Next we will create three users and assign each a job title.  
	1. In the Auth0 Dashboard go to the users tab.  If you do not have three users create them. Naming them a variation of reader, writer, and deleter would be helpful.  
	2. Go to User Details for the reader user and scroll down to app_metadata.  Enter the following property in the app_metadata json "jobTitle": "ItemReader".  
	3. Go to User Details for the writer user and scroll down to app_metadata.  Enter the following property in the app_metadata json "jobTitle": "ItemWriter".  
	4. Go to User Details for the deleter user and scroll down to app_metadata.  Enter the following property in the app_metadata json "jobTitle": "ItemDeleter".  
4.  Lastly we will create a rule to set the users scope in the access token returned upon a successful login.  
	1. In the Auth0 Dashboard go to the rules tab. Click create rule. 
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
  
		context.accessToken.scope = [scope, 'openid'];
		callback(null, user, context);
	}
	```
5.  (Optional) The users's job title can be included in the id_token if you desire.  
	1. In the Auth0 Dashboard go to the rules tab. Click create rule. 
	2. Click empty rule.
	3. Name your rule and insert this javascript into the below rule.  The script retrieves the job title we created in step 3 from the app_metadata property. It is then added to a (namespaced) property calleld jobTitle.  
	
	```javascript
	function (user, context, callback) {
	  if (context.request.geoip) {
		context.idToken['https://auth0sample/jobTitle'] = user.app_metadata.jobTitle;
	  }

	  callback(null, user, context);
	}
	```



## AngularJS Setup

The application in this sample is an AngularJS application setup similarly to the 
[AngularJS: Login](https://auth0.com/docs/quickstart/spa/angularjs/01-login) tutorial. 

1.  In the app.js file angularAuth0Provider.init should be set to the clientId and domain we configured for the application in the dashboard. The redirectUri should match the url we set as the Allowed Callback Url.  Audience should be the identifier we configured for our Api.  
```javascript
 angularAuth0Provider.init({
      clientID: 'app client id',
      domain: 'app domain',
      responseType: 'token id_token',
      audience: 'api Identifier',
      redirectUri: 'http://localhost:3000/callback',
      scope: 'openid'
    });
```
2.  This example adds an app.constants.es6 file that includes a constant defining the url to our mock api service.  If you are running the api at a different url it should be entered here.  
3.  This sample is dependent on several packages (see index.html).  To download them run npm install from a command prompt in the App folder.
	```
	npm install
	```
4.  To run the app simply enter node server in the same command line.
	```
	node server
	```

	
## AngularJS Walkthrough

Run the app and login with one of the users you created then check browser local storage for your token.  In chrome you can do this by opening developer tools with the F12 key and going to the Application tab. Find your application under local storage and you should see an entry for access_token.  To decode the token and view the contents you can go to https://jwt.io and paste the token into the encoded textbox.  You should see your scope in the decoded payload json like below
```json
{
  ...
  ...
  "scope": "read:items"
}
``` 



If you created the rule to add the user's job title to the id token you can retrieve and view it the same way you did the access_token.  It should be visible in local storage as id_token and contain the job title.
```json
{
  "https://auth0sample/jobTitle": "ItemReader",
  ...
  ...
}
```
The getUserItemsScope function in the authService (app/auth/auth.service.js) demonstrates how you can use a library to decode the token and access the scope.  
```javascript
var tokenPayload = jwtHelper.decodeToken(localStorage.getItem('access_token'));
```
In this sample the scope is being read from the token and buttons corresponding to a scope the user does not possess disabled.  See the item buttons component controller and html for details. All buttons can be enabled by clicking the checkbox.

Upon clicking one of the buttons a call is made to our mock api service.  An AngularJS interceptor is used to attach the token to any outgoing requests.
#### app.js
```javascript
    //register the interceptor that will add token to outgoing requests.
    $httpProvider.interceptors.push('TokenInterceptor');
```

#### tokenInterceptor.es6
```javascript
(() => {        
    'use strict';
    angular.module('app').factory('TokenInterceptor',TokenInterceptor);

    TokenInterceptor.$inject = ["authService"];
        
    function TokenInterceptor(authService) {
        //when token available adds an Authorization header to each outbound request
        //with value: Bearer access_token
        var interceptor = { request: request };

        function request(config) {
            if (authService.isAuthenticated()){
                config.headers.authorization = 'Bearer ' + localStorage.getItem('access_token');
            }
            return config;
        }
        
        return interceptor;
    }
}
)();
```

Upon clicking any of the buttons you will receive a response indicating either the user had the appropriate scope for the corresponding endpoint, or they lacked authorization.

## Web API Service Setup and Walkthrough
The mock WebApi service follows the [ASP.NET Web API (OWIN): Authorization](https://auth0.com/docs/quickstart/backend/webapi-owin/01-authorization) tutorial exactly.  That tutorial explains how the API is setup and how the [ScopeAuthorize] attribute is added to endpoints to authorize only users with the appropriate scope.  


 
# Refresh Token






