# Media Permissions

## Introduction

This application implements the logic to check whether a user has access to media assets. It is used by a media streaming provider.

## Data Model

### User Properties

A user has the following properties that are relevant for this application (users have more properties, but these are the ones that are relevant for this application):

* `is_active`: A boolean value indicating whether the user is active.
* `streaming_packages`: A list of strings with identifiers of the streaming packages the user is currently subscribed to.
* `country_iso_code`: A string value indicating the ISO code of the country the user is streaming from.

### Media Properties

* `title`: A string value indicating the title of the media asset.
* `series`: An optional string value indicating the title of the series of the media asset. If undefined, the media asset is not part of a series.
* `category`: A string value indicating the category of the media asset (e.g. "movie", "series", "documentary", "news", "sports", "adult", etc.).

### Permissions

Permissions are defined by the following data structure:

* `media_filter`: An object identifying media assets. All fields are optional.
  * `series`: An optional series filter. If undefined, the series filter is not applied.
  * `category`: An optional category filter. If undefined, the category filter is not applied.

* `user_filter`: An object identifying users. All fields are optional.
  * `is_active`: An optional active user filter. If undefined, the active user filter is not applied.
  * `streaming_package`: An optional streaming package filter. If undefined, the streaming package filter is not applied.
  * `country_iso_code`: An optional country ISO code filter. If undefined, the country ISO code filter is not applied.

* `access`: Either _allowed_ or _denied_.

Media asset permissions work like rules in a firewall. To determine whether a user has access to a media asset, the application checks all permissions in the order of their definition. The last permission that matches is used to determine the access.

The following example denies access to all inactive users. Then, it allows access to "4K Testvideos" for all users.

```yaml
- media_filter:
  user_filter:
    is_active: false
  access: "denied"
- media_filter:
    series: "4K Testvideos"
  user_filter:
  access: "allowed"
```

The following example denies access to a specific series all users. Then, it allows access to the media for all users in Austria.

```yaml
- media_filter:
    series: "ORF - Zeit im Bild 2"
  user_filter:
  access: "denied"
- media_filter:
    series: "ORF - Zeit im Bild 2"
  user_filter:
    country_iso_code: "AT"
  access: "allowed"
```

The following example denies access to all movies for all users. Then, it allows access to all movies for users with the streaming package "premium".

```yaml
- media_filter:
    category: "movie"
  user_filter:
  access: "denied"
- media_filter:
    category: "movie"
  user_filter:
    streaming_package: "premium"
  access: "allowed"
```
