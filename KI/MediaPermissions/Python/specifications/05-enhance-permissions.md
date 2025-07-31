# Enhance Permissions

As you can see in [permissions_prod.yaml](permissions_prod.yaml), defining similar permissions for multiple media assets (e.g. *Game of Thrones* and *House of Cards*) is a bit cumbersome. One needs to setup individual rules for each series.

We want to change the media permissions (see [media_permissions.py](media_permissions.py)) to allow defining permissions for multiple series and/or categories at once. It is not necessary to support both, single string and list of strings. If the user wants to define a rule for e.g. a single category, he uses a list of strings with a single element. An empty list means that the rule is not applied. The series and category filters should remain optional.

Here is an example. Until now, we have to define rules like this:

```yaml
- media_filter:
    series: "Game of Thrones"
  user_filter: {}
  access: "denied"
- media_filter:
    series: "Game of Thrones"
  user_filter:
    streaming_package: "premium"
  access: "allowed"

- media_filter:
    series: "House of Cards"
  user_filter: {}
  access: "denied"
- media_filter:
    series: "House of Cards"
  user_filter:
    streaming_package: "premium"
  access: "allowed"
```

In the future, the following should be possible:

```yaml
- media_filter:
    series: ["Game of Thrones", "House of Cards"]
  user_filter: {}
  access: "denied"
- media_filter:
    series: ["Game of Thrones", "House of Cards"]
  user_filter:
    streaming_package: "premium"
  access: "allowed"
```

This sample only demonstrates the new syntax with `series`. However, the same logic must be available for `category` as well.

## Checklist

* You have to update [media_permissions.py](media_permissions.py) to support the new syntax.
* You have to adjust the logic in [permission_validation.py](permission_validation.py) to support the new syntax (including unit tests in [test_permission_validation.py](test_permission_validation.py)).
* You have to update the YAML files [permissions_prod.yaml](permissions_prod.yaml) and [sample_permissions.yaml](sample_permissions.yaml) to use the new syntax.

## Notes

* Backwards compatibility is not necessary.
* We do not need additional unit tests. Just adjust the existing ones.
