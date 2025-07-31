#!/usr/bin/env python3
"""
Unit tests for permission validation functionality.

Tests the permission validation logic with various user and media combinations
based on the examples from the introduction.
"""

import unittest
from media_permissions import Permission, MediaFilter, UserFilter
from permission_validation import User, Media, validate_permission


class TestPermissionValidation(unittest.TestCase):
    """Test cases for permission validation functionality."""
    
    def setUp(self):
        """Set up test fixtures with users and media assets."""
        # Test users
        self.active_premium_user_at = User(
            is_active=True,
            streaming_packages=["basic", "premium"],
            country_iso_code="AT"
        )
        
        self.inactive_user_de = User(
            is_active=False,
            streaming_packages=["basic"],
            country_iso_code="DE"
        )
        
        self.active_basic_user_us = User(
            is_active=True,
            streaming_packages=["basic"],
            country_iso_code="US"
        )
        
        self.active_basic_user_at = User(
            is_active=True,
            streaming_packages=["basic"],
            country_iso_code="AT"
        )
        
        # Test media assets
        self.testvideos_series = Media(
            title="Sample Test Video",
            series="4K Testvideos",
            category="documentary"
        )
        
        self.orf_news = Media(
            title="Evening News",
            series="ORF - Zeit im Bild 2",
            category="news"
        )
        
        self.premium_movie = Media(
            title="Action Hero",
            series=None,
            category="movie"
        )
        
        self.regular_documentary = Media(
            title="Nature Documentary",
            series=None,
            category="documentary"
        )

    def test_example_1_inactive_users_and_testvideos(self):
        """Test example 1: Deny inactive users, allow 4K Testvideos for all."""
        permissions = [
            Permission(
                media_filter=MediaFilter(),
                user_filter=UserFilter(is_active=False),
                access="denied"
            ),
            Permission(
                media_filter=MediaFilter(series="4K Testvideos"),
                user_filter=UserFilter(),
                access="allowed"
            )
        ]
        
        # Active user accessing 4K Testvideos should be allowed
        result = validate_permission(self.active_premium_user_at, self.testvideos_series, permissions)
        self.assertEqual(result, "allowed")
        
        # Inactive user accessing 4K Testvideos should be allowed (last matching rule)
        result = validate_permission(self.inactive_user_de, self.testvideos_series, permissions)
        self.assertEqual(result, "allowed")
        
        # Inactive user accessing other content should be denied
        result = validate_permission(self.inactive_user_de, self.regular_documentary, permissions)
        self.assertEqual(result, "denied")
        
        # Active user accessing other content should be denied (no matching allow rule)
        result = validate_permission(self.active_premium_user_at, self.regular_documentary, permissions)
        self.assertEqual(result, "denied")

    def test_example_2_orf_series_austria_only(self):
        """Test example 2: Deny ORF series for all, allow for Austrian users."""
        permissions = [
            Permission(
                media_filter=MediaFilter(series="ORF - Zeit im Bild 2"),
                user_filter=UserFilter(),
                access="denied"
            ),
            Permission(
                media_filter=MediaFilter(series="ORF - Zeit im Bild 2"),
                user_filter=UserFilter(country_iso_code="AT"),
                access="allowed"
            )
        ]
        
        # Austrian user accessing ORF series should be allowed
        result = validate_permission(self.active_premium_user_at, self.orf_news, permissions)
        self.assertEqual(result, "allowed")
        
        # Austrian user with basic package should also be allowed
        result = validate_permission(self.active_basic_user_at, self.orf_news, permissions)
        self.assertEqual(result, "allowed")
        
        # Non-Austrian user accessing ORF series should be denied
        result = validate_permission(self.active_basic_user_us, self.orf_news, permissions)
        self.assertEqual(result, "denied")
        
        # Austrian user accessing other content should be denied (no permission rule matches)
        result = validate_permission(self.active_premium_user_at, self.regular_documentary, permissions)
        self.assertEqual(result, "denied")

    def test_example_3_movies_premium_only(self):
        """Test example 3: Deny movies for all, allow for premium users."""
        permissions = [
            Permission(
                media_filter=MediaFilter(category="movie"),
                user_filter=UserFilter(),
                access="denied"
            ),
            Permission(
                media_filter=MediaFilter(category="movie"),
                user_filter=UserFilter(streaming_package="premium"),
                access="allowed"
            )
        ]
        
        # Premium user accessing movie should be allowed
        result = validate_permission(self.active_premium_user_at, self.premium_movie, permissions)
        self.assertEqual(result, "allowed")
        
        # Basic user accessing movie should be denied
        result = validate_permission(self.active_basic_user_us, self.premium_movie, permissions)
        self.assertEqual(result, "denied")
        
        # Premium user accessing non-movie content should be denied (no matching rule)
        result = validate_permission(self.active_premium_user_at, self.regular_documentary, permissions)
        self.assertEqual(result, "denied")
        
        # Basic user accessing non-movie content should be denied (no matching rule)
        result = validate_permission(self.active_basic_user_us, self.regular_documentary, permissions)
        self.assertEqual(result, "denied")

    def test_no_permissions_defaults_to_denied(self):
        """Test that when no permissions are provided, access defaults to denied."""
        permissions = []
        
        result = validate_permission(self.active_premium_user_at, self.testvideos_series, permissions)
        self.assertEqual(result, "denied")

    def test_no_matching_permissions_defaults_to_denied(self):
        """Test that when no permissions match, access defaults to denied."""
        permissions = [
            Permission(
                media_filter=MediaFilter(series="Non-existent Series"),
                user_filter=UserFilter(),
                access="allowed"
            )
        ]
        
        result = validate_permission(self.active_premium_user_at, self.testvideos_series, permissions)
        self.assertEqual(result, "denied")

    def test_multiple_matching_permissions_last_wins(self):
        """Test that when multiple permissions match, the last one determines access."""
        permissions = [
            Permission(
                media_filter=MediaFilter(category="documentary"),
                user_filter=UserFilter(),
                access="allowed"
            ),
            Permission(
                media_filter=MediaFilter(),
                user_filter=UserFilter(is_active=True),
                access="denied"
            ),
            Permission(
                media_filter=MediaFilter(category="documentary"),
                user_filter=UserFilter(country_iso_code="AT"),
                access="allowed"
            )
        ]
        
        # Austrian user should be allowed (last matching rule)
        result = validate_permission(self.active_premium_user_at, self.regular_documentary, permissions)
        self.assertEqual(result, "allowed")
        
        # US user should be denied (second rule matches and is last for this user)
        result = validate_permission(self.active_basic_user_us, self.regular_documentary, permissions)
        self.assertEqual(result, "denied")

    def test_complex_filter_combinations(self):
        """Test complex combinations of media and user filters."""
        permissions = [
            Permission(
                media_filter=MediaFilter(category="movie", series=None),
                user_filter=UserFilter(is_active=True, streaming_package="premium"),
                access="allowed"
            )
        ]
        
        # This should not match because series=None in filter doesn't match series="4K Testvideos"
        result = validate_permission(self.active_premium_user_at, self.testvideos_series, permissions)
        self.assertEqual(result, "denied")
        
        # This should match because movie has series=None
        result = validate_permission(self.active_premium_user_at, self.premium_movie, permissions)
        self.assertEqual(result, "allowed")


if __name__ == "__main__":
    unittest.main()