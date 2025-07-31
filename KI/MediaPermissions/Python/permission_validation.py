from pydantic import BaseModel
from typing import List, Optional, Literal
from media_permissions import Permission


class User(BaseModel):
    """User data model for permission validation."""
    is_active: bool
    streaming_packages: List[str]
    country_iso_code: str


class Media(BaseModel):
    """Media asset data model for permission validation."""
    title: str
    series: Optional[str] = None
    category: str


def validate_permission(user: User, media: Media, permissions: List[Permission]) -> Literal["allowed", "denied"]:
    """
    Validate if a user has access to a media asset based on permissions.
    
    Permissions work like firewall rules - they are checked in order and the
    last matching permission determines the access.
    
    Args:
        user: User data to check
        media: Media asset to check access for
        permissions: List of permission rules to evaluate
        
    Returns:
        "allowed" or "denied" based on the last matching permission
    """
    # Default to denied if no permissions match
    last_match = "denied"
    
    for permission in permissions:
        # Check if this permission matches the media asset
        media_matches = True
        
        # Check series filter
        if permission.media_filter.series is not None:
            if media.series != permission.media_filter.series:
                media_matches = False
                
        # Check category filter
        if permission.media_filter.category is not None:
            if media.category != permission.media_filter.category:
                media_matches = False
        
        # Check if this permission matches the user
        user_matches = True
        
        # Check is_active filter
        if permission.user_filter.is_active is not None:
            if user.is_active != permission.user_filter.is_active:
                user_matches = False
                
        # Check streaming_package filter
        if permission.user_filter.streaming_package is not None:
            if permission.user_filter.streaming_package not in user.streaming_packages:
                user_matches = False
                
        # Check country_iso_code filter
        if permission.user_filter.country_iso_code is not None:
            if user.country_iso_code != permission.user_filter.country_iso_code:
                user_matches = False
        
        # If both media and user match, this permission applies
        if media_matches and user_matches:
            last_match = permission.access
    
    return last_match