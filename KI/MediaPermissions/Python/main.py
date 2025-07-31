#!/usr/bin/env python3
"""
Demo usage of the media permissions deserialization functionality.
"""

from media_permissions import load_permissions_from_yaml, Permission
from permission_validation import User, Media, validate_permission
from pydantic import ValidationError
import yaml


def main():
    """Demo the deserialization of permissions from YAML file."""
    print("Media Permissions Deserialization Demo")
    print("=" * 40)
    
    # Load permissions from the sample YAML file
    try:
        permissions = load_permissions_from_yaml("sample_permissions.yaml")
        print(f"✅ Successfully loaded {len(permissions)} permissions from YAML file")
        print()
        
        # Display each permission
        for i, permission in enumerate(permissions, 1):
            print(f"Permission {i}:")
            print(f"  Media Filter:")
            if permission.media_filter.series:
                print(f"    Series: {permission.media_filter.series}")
            if permission.media_filter.category:
                print(f"    Category: {permission.media_filter.category}")
            if not permission.media_filter.series and not permission.media_filter.category:
                print(f"    (No media filters)")
                
            print(f"  User Filter:")
            if permission.user_filter.is_active is not None:
                print(f"    Is Active: {permission.user_filter.is_active}")
            if permission.user_filter.streaming_package:
                print(f"    Streaming Package: {permission.user_filter.streaming_package}")
            if permission.user_filter.country_iso_code:
                print(f"    Country: {permission.user_filter.country_iso_code}")
            if (permission.user_filter.is_active is None and 
                not permission.user_filter.streaming_package and 
                not permission.user_filter.country_iso_code):
                print(f"    (No user filters)")
                
            print(f"  Access: {permission.access}")
            print()
        
        # Demo permission validation functionality
        print("\n" + "=" * 40)
        print("Permission Validation Demo")
        print("=" * 40)
        
        # Create test users
        active_premium_user_at = User(
            is_active=True,
            streaming_packages=["basic", "premium"],
            country_iso_code="AT"
        )
        
        inactive_user_de = User(
            is_active=False,
            streaming_packages=["basic"],
            country_iso_code="DE"
        )
        
        active_basic_user_us = User(
            is_active=True,
            streaming_packages=["basic"],
            country_iso_code="US"
        )
        
        # Create test media assets
        testvideos_series = Media(
            title="Sample Test Video",
            series="4K Testvideos",
            category="documentary"
        )
        
        orf_news = Media(
            title="Evening News",
            series="ORF - Zeit im Bild 2",
            category="news"
        )
        
        premium_movie = Media(
            title="Action Hero",
            series=None,
            category="movie"
        )
        
        # Test cases
        test_cases = [
            (active_premium_user_at, testvideos_series, "Active premium user from AT accessing 4K Testvideos"),
            (inactive_user_de, testvideos_series, "Inactive user from DE accessing 4K Testvideos"),
            (active_premium_user_at, orf_news, "Active premium user from AT accessing ORF news"),
            (active_basic_user_us, orf_news, "Active basic user from US accessing ORF news"),
            (active_premium_user_at, premium_movie, "Active premium user from AT accessing movie"),
            (active_basic_user_us, premium_movie, "Active basic user from US accessing movie"),
        ]
        
        for user, media, description in test_cases:
            result = validate_permission(user, media, permissions)
            status_icon = "✅" if result == "allowed" else "❌"
            print(f"{status_icon} {description}: {result.upper()}")
            
    except FileNotFoundError as e:
        print(f"❌ Error: {e}")
    except ValidationError as e:
        print(f"❌ Validation Error: {e}")
    except yaml.YAMLError as e:
        print(f"❌ YAML Error: {e}")
    except Exception as e:
        print(f"❌ Unexpected Error: {e}")


if __name__ == "__main__":
    main()
