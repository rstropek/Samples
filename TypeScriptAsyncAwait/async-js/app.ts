import * as async from 'async';
import * as mongodb from 'mongodb';

/*
    Intended algorithm (pseudocode):

    Open Database
    Read all persons with first name "John"
    For each person:
        If Person is customer:
            Read customer details for person
            Print person and customer details
        Else:
            Read supplier details for person
            Print person and supplier details

    Note that there would be other ways to join collections
    in mongoDB (e.g. $lookup). However, this example should
    demonstrate the problem with callbacks.
*/

/*
 * This code is slightly better than the version in
 * ../plain-js-callbacks/app.ts. It uses async.js to make async
 * code more readable. For details see http://caolan.github.io/async/.
 */

let database: mongodb.Db;
let client: mongodb.MongoClient;
async.waterfall(
    [
      // Open Database
      callback =>
          mongodb.MongoClient.connect('mongodb://localhost:27017', callback),

      // Read all persons with first name "John"
      (cli, callback) => {
        client = cli;
        database = client.db('demo');
        database.collection('Person', callback);
      },
      (coll, callback) => coll.find({firstName: 'John'}).toArray(callback),
      (res, callback) => {
        // In order to call the callback after we are done, we have to use a
        // counter
        var counter = res.length;
        var markOneAsProcessed = () => {
          if (--counter == 0) callback();
        };

        // For each person
        for (var i = 0; i < res.length; i++) {
          var p = res[i];

          // If Person is customer
          if (p.isCustomer) {
            async.waterfall(
                [
                  // Read customer details for person
                  callback => database.collection('Customer', callback),
                  (custColl, callback) =>
                      custColl.findOne({_id: p.customerId}, callback),

                  // Print person and customer details
                  (cust, callback) => {
                    console.log(`John ${cust.lastName} works for ${cust.customerName}.`);
                    callback();
                  },
                ],
                (err, result) => markOneAsProcessed());
          } else {
            async.waterfall(
                [
                  // Read supplier details for person
                  callback => database.collection('Supplier', callback),
                  (supplColl, callback) =>
                      supplColl.findOne({_id: p.supplierId}, callback),

                  // Print person and supplier details
                  (suppl, callback) => {
                    console.log(`John ${suppl.lastName} works for ${suppl.supplierName}.`);
                    callback();
                  },
                ],
                (err, result) => markOneAsProcessed());
          }
        }
      }
    ],
    (err, result) => client.close());
