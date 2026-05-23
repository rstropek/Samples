import yaml
from pydantic import BaseModel, ValidationError
from typing import List, Optional, Literal
from pathlib import Path

class MediaFilter(BaseModel):
    """Filter for media assets based on series and category."""
    series: Optional[str] = None
    category: Optional[str] = None


class UserFilter(BaseModel):
    """Filter for users based on activity status, streaming package, and country."""
    is_active: Optional[bool] = None
    streaming_package: Optional[str] = None
    country_iso_code: Optional[str] = None



class Permission(BaseModel):
    """Permission rule defining access for media assets and users."""
    media_filter: MediaFilter
    user_filter: UserFilter
    access: Literal["allowed", "denied"]


def load_permissions_from_yaml(file_path: str) -> List[Permission]:
    """
    Load and deserialize permissions from a YAML file.
    
    Args:
        file_path: Path to the YAML file containing permissions
        
    Returns:
        List of Permission objects
        
    Raises:
        FileNotFoundError: If the YAML file doesn't exist
        ValidationError: If the YAML structure doesn't match the expected format
        yaml.YAMLError: If the YAML file is malformed
    """
    yaml_path = Path(file_path)
    
    if not yaml_path.exists():
        raise FileNotFoundError(f"YAML file not found: {file_path}")
    
    try:
        with open(yaml_path, 'r', encoding='utf-8') as file:
            yaml_data = yaml.safe_load(file)
            
        if not isinstance(yaml_data, list):
            raise ValidationError("YAML file must contain a list of permissions")
            
        permissions = []
        for item in yaml_data:
            permission = Permission(**item)
            permissions.append(permission)
            
        return permissions
        
    except yaml.YAMLError as e:
        raise yaml.YAMLError(f"Error parsing YAML file: {e}")
    except ValidationError as e:
        raise ValidationError(f"Error validating permission data: {e}")
