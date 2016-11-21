#pragma once

#include "ArrayList.h"

class Entity;
class Scene;

namespace RiverEngine
{
	class PhysicsEngine
	{
	public:
		PhysicsEngine();
		~PhysicsEngine();
		bool Update(float dt);
		bool RefreshEntities();
		Scene* owner = 0;
	private:
		ArrayList<Entity*> m_entityList;
	};
}