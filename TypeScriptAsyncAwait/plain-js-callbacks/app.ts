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
 * Note that the following algorithm demonstrates an ANTI-PATTERN.
 * Code like this is called "Callback Hell" (for more details see
 * e.g. http://callbackhell.com/).
 */

// Open Database
mongodb.MongoClient.connect("mongodb://10.0.75.2:27017/demo", (err, db) => {
    // Read all persons with first name "John"
    db.collection("Person", (err, coll) => {
        coll.find({ firstName: "John" }).toArray((err, res) => {
            // In order to close DB after we are done, we have to use a counter
            var counter = res.length;
            var closedb = () => { if (--counter == 0) db.close(); };

            // For each person
            for (var i = 0; i < res.length; i++) {
                var p = res[i];

                // If Person is customer
                if (p.isCustomer) {
                    // Read customer details for person
                    db.collection("Customer", (err, custColl) => {
                        custColl.findOne({ id: p.customerId }, (err, cust) => {
                            // Print person and customer details
                            console.log(`John ${p.lastName} works for ${cust.customerName}.`);

                            // Call helper function to close DB if done
                            closedb();
                        });
                    });
                } else {
                    // Read supplier details for person
                    db.collection("Supplier", (err, supplColl) => {
                        supplColl.findOne({ id: p.supplierId }, (err, suppl) => {
                            // Print person and supplier details
                            console.log(`John ${p.lastName} works for ${suppl.supplierName}.`);

                            // Call helper function to close DB if done
                            closedb();
                        });
                    });
                }
            }
        });
    });
});
