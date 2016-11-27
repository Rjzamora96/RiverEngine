#pragma once

#include "ArrayList.h"
#include "BoxCollider.h"
#include "Vector2.h"

namespace RiverEngine
{
	class Entity;
	class Scene;

	class PhysicsEngine
	{
	public:
		PhysicsEngine();
		~PhysicsEngine();
		bool Update(float dt);
		bool RefreshEntities();
		bool DoOverlap(Vector2 l1, Vector2 r1, Vector2 l2, Vector2 r2);
		Scene* owner = 0;
	private:
		ArrayList<Entity*> m_entityList;
		ArrayList<BoxCollider*> m_boxColliders;
	};
}