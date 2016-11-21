#pragma once
#include "Rectangle.h"
#include "Component.h"

namespace RiverEngine
{
	class BoxCollider : public Component
	{
	public:
		BoxCollider();
		~BoxCollider();
		Rectangle rect;
	};
}

