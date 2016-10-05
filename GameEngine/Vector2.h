#pragma once

#include <iostream>
#include <math.h>
#include <exception>

struct Vector2
{
	float x;
	float y;
	Vector2(float const x = 0, float const y = 0) : x(x), y(y) {}
	float Length() const
	{
		return sqrt(pow(this->x, 2) + pow(this->y, 2));
	}

	float LengthSquared() const
	{
		return pow(this->Length(), 2);
	}

	operator float*()
	{
		return &x;
	}

	inline Vector2 Normalized() const
	{
		float scalar = (this->Length() != 0) ? (1.0f / this->Length()) : 0;
		return Vector2(this->x * scalar, this->y * scalar);
	}

	inline Vector2 PerpCW() const
	{
		return Vector2(this->y, -(this->x));
	}

	inline Vector2 PerpCCW() const
	{
		return Vector2(-(this->y), this->x);
	}

	inline float Dot(const Vector2& right) const
	{
		return (this->x * right.x) + (this->y * right.y);
	}

	inline float Cross(const Vector2& right) const
	{
		return (this->x * right.y) - (this->y * right.x);
	}
};

inline Vector2 operator+(const Vector2& left, const Vector2& right)
{
	return Vector2(left.x + right.x, left.y + right.y);
}

inline Vector2 operator-(const Vector2& left, const Vector2& right)
{
	return Vector2(left.x + -(right.x), left.y + -(right.y));
}

inline Vector2 operator*(float const scalar, const Vector2& right)
{
	return Vector2(scalar * right.x, scalar * right.y);
}

inline Vector2 operator*(const Vector2& left, float const scalar)
{
	return Vector2(left.x * scalar, left.y * scalar);
}

inline Vector2 operator/(const Vector2& left, float const scalar)
{
	return Vector2(left.x / scalar, left.y / scalar);
}

inline Vector2 LERP(const Vector2& left, const Vector2& right, float const scalar)
{
	return ((1.0f - scalar) * left) + (scalar * right);
}

inline std::ostream& operator<<(std::ostream& out, const Vector2& vector)
{
	out << "X: " << vector.x << " Y: " << vector.y;
	return out;
}

inline float Dot(const Vector2& left, const Vector2& right)
{
	return (left.x * right.x) + (left.y * right.y);
}

inline float Cross(const Vector2& left, const Vector2& right)
{
	return (left.x * right.y) - (left.y * right.x);
}