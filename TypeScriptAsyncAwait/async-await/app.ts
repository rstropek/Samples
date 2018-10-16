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
*/

/*
 * This code sample shows how similar the TypeScript code is to
 * our specification in pseudocode shown above. This is done
 * using TypeScript's async/await feature.
 *
 * Recommendation: Compile this TypeScript file to ECMAScript and
 * make yourself familiar with the generated code.
 */

async function run() {
  // Open Database
  var client = await mongodb.MongoClient.connect('mongodb://10.0.75.2:27017', { useNewUrlParser: true });
  var db = client.db('demo');

  // Read all persons with first name "John"
  var persons =
      await db.collection('Person').find({firstName: 'John'}).toArray();

  // For each person
  for (var i = 0; i < persons.length; i++) {
    var p = persons[i];

    // If Person is customer
    if (p.isCustomer) {
      // Read customer details for person
      var cust = await db.collection('Customer').findOne({_id: p.customerId});

      // Print person and customer details
      console.log(`John ${p.lastName} works for ${cust.customerName}.`);
    } else {
      // Read supplier details for person
      var suppl = await db.collection('Supplier').findOne({_id: p.supplierId});

      // Print person and supplier details
      console.log(`John ${p.lastName} works for ${suppl.supplierName}.`);
    }
  }

  client.close();
}

run();
