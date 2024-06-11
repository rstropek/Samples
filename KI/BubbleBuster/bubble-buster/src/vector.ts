/**
 * Class representing a 2D vector with usual operations
 */
export class Vector {
    /**
     * Create a new vector
     * 
     * @param x - The x component of the vector
     * @param y - The y component of the vector
     */
    constructor(public x: number, public y: number) {}

    /**
     * Add another vector to this vector
     * 
     * @param other - The other vector to add
     * @returns A new vector that is the sum of this vector and the other vector
     */
    add(other: Vector): Vector {
        return new Vector(this.x + other.x, this.y + other.y);
    }

    /**
     * Subtract another vector from this vector
     * 
     * @param other - The other vector to subtract
     * @returns A new vector that is the difference of this vector and the other vector
     */
    sub(other: Vector): Vector {
        return new Vector(this.x - other.x, this.y - other.y);
    }

    /**
     * Multiply this vector by a scalar
     * 
     * @param scalar - The scalar to multiply by
     * @returns A new vector that is the product of this vector and the scalar
     */
    mul(scalar: number): Vector {
        return new Vector(this.x * scalar, this.y * scalar);
    }

    /**
     * Divide this vector by a scalar
     * 
     * @param scalar - The scalar to divide by
     * @returns A new vector that is the quotient of this vector and the scalar
     */
    div(scalar: number): Vector {
        return new Vector(this.x / scalar, this.y / scalar);
    }

    /**
     * Get the magnitude of this vector
     * 
     * @returns The magnitude of this vector
     */
    mag(): number {
        return Math.sqrt(this.x * this.x + this.y * this.y);
    }

    /**
     * Normalize this vector
     * 
     * @returns A new vector that is the normalized version of this vector
     */
    normalize(): Vector {
        return this.div(this.mag());
    }

    /**
     * Get the dot product of this vector and another vector
     * 
     * @param other - The other vector to get the dot product with
     * @returns The dot product of this vector and the other vector
     */
    dot(other: Vector): number {
        return this.x * other.x + this.y * other.y;
    }

    rotate(angle: number): Vector {
        const x = this.x * Math.cos(angle) - this.y * Math.sin(angle);
        const y = this.x * Math.sin(angle) + this.y * Math.cos(angle);
        return new Vector(x, y);
    }
}
