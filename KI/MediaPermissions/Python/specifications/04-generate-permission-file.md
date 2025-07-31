# Generate Permission File

I need a permission file according to [01-introduction.md](01-introduction.md) and [02-deserialization.md](02-deserialization.md). Call it *permissions_prod.yaml*.

It must contain the following rules:

* Disables users must only see the series _Testvideos_.
* The series _ORF Lokalsender_ is only available in Austria.
* The category _movie_ is only available in the streaming package _premium_.
* The following special series are only available for users with the streaming package _premium_:
  * Game of Thrones
  * House of Cards
* The category *sport_live* is available for users with the streaming package _sports_ and _premium_.
