#pragma once

#include "Component.h"
#include "Vector2.h"

class Transform : public Component
{
public:
	Transform();
	Vector2 getPosition() const { return m_position; }
	void setPosition(Vector2 v) { m_position = v; }
	float getRotation() const { return m_rotation; }
	void setRotation(float f) { m_rotation = f; }
private:
	Vector2 m_position;
	float m_rotation;
};

Transform::Transform() : m_position(Vector2()), m_rotation(0)
{
}