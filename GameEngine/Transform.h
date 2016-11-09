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
		void SetPosition(Vector2 vector)
		{
			localPosition->x = vector.x;
			localPosition->y = vector.y;
			Entity* parent = owner->parent;
			if (parent != 0)
			{
				Transform* parentTransform = parent->GetComponentByType<Transform>();
				if (parentTransform != 0)
				{
					localPosition->x = (vector.x - parentTransform->position->x)/parentTransform->scale;
					localPosition->y = (vector.y - parentTransform->position->y)/parentTransform->scale;
				}
			}
		}
		Vector2* GetPosition(){ return position; }
		bool Update(float dt);
	};
}