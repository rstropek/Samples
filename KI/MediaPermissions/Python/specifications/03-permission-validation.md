# Permission Validation

Create a new python file _permission_validation.py_ that can find out if a given user has access to a given media asset. The general logic is described in [01-introduction.md](01-introduction.md).

Perform the following tasks:

* Create classes that hold the user and media asset data (defined in [01-introduction.md](01-introduction.md)).
* Create a function that receives user data, media data, and a list of permissions (see also [02-deserialization.md](02-deserialization.md) and [media_permissions.py](media_permissions.py)). It returns either _allowed_ or _denied_.

Extend the example usage in _main.py_ to demonstrate the usage of the permission validation function.

Create unit tests with the test cases defined in [01-introduction.md](01-introduction.md).
