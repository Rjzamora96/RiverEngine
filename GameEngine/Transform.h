#pragma once

#include "Component.h"
#include "Vector2.h"
namespace RiverEngine
{
	class Transform : public Component
	{
	public:
		Transform() : position(new Vector2()), localPosition(new Vector2()), rotation(0), scale(1) {}
		Vector2* localPosition;
		Vector2* position;
		float localRotation;
		float rotation;
		float localScale;
		float scale;
		bool Update(float dt);
	};
}