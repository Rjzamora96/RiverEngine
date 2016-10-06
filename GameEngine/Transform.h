#pragma once

#include "Component.h"
#include "Vector2.h"
namespace RiverEngine
{
	class Transform : public Component
	{
	public:
		Transform() : position(new Vector2()), rotation(0) {}
		Vector2* position;
		float rotation;
	};
}