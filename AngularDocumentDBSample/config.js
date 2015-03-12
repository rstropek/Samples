var config = {}

config.host = process.env.HOST || "https://your-host.documents.azure.com:443/";
config.authKey = process.env.AUTH_KEY || "your-documentdb-key";
config.databaseId = "urXXX=="; // Your database resource ID
config.collectionId = "ur1kXXXXXXX="; // Your collection resource ID
config.collectionPath = "dbs/" + config.databaseId + "/colls/" + config.collectionId + "/";
config.infoMailReceipients = [{ "email": "demo@nowhere.com" }];
config.recaptchaSecret = "your-recaptcha-secret";
config.mandrillKey = "your-mandrill-key";

module.exports = config;
