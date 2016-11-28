#include "PhysicsEngine.h"
#include "Scene.h"
#include "BoxCollider.h"
#include "Entity.h"
#include "Transform.h"
namespace RiverEngine
{
	PhysicsEngine::PhysicsEngine()
	{
	}


	PhysicsEngine::~PhysicsEngine()
	{
	}

	bool PhysicsEngine::Update(float dt)
	{
		for (int i = 0; i < m_boxColliders.Count() - 1; i++)
		{
			for (int k = i + 1; k < m_boxColliders.Count(); k++)
			{
				Vector2* currentPosition = m_boxColliders[i]->owner->transform->position;
				Vector2 l1(currentPosition->x + (m_boxColliders[i]->rect.x * m_boxColliders[i]->owner->transform->scale), currentPosition->y + (m_boxColliders[i]->rect.y * m_boxColliders[i]->owner->transform->scale));
				Vector2 r1(l1.x + (m_boxColliders[i]->rect.width * m_boxColliders[i]->owner->transform->scale), l1.y + (m_boxColliders[i]->rect.height * m_boxColliders[i]->owner->transform->scale));
				Vector2* otherPosition = m_boxColliders[k]->owner->transform->position;
				Vector2 l2(otherPosition->x + (m_boxColliders[k]->rect.x * m_boxColliders[k]->owner->transform->scale), otherPosition->y + (m_boxColliders[k]->rect.y * m_boxColliders[k]->owner->transform->scale));
				Vector2 r2(l2.x + (m_boxColliders[k]->rect.width * m_boxColliders[k]->owner->transform->scale), l2.y + (m_boxColliders[k]->rect.height * m_boxColliders[k]->owner->transform->scale));
				if (DoOverlap(l1, r1, l2, r2))
				{
					luabridge::LuaRef message = luabridge::LuaRef::newTable(Scene::GetState());
					m_boxColliders[i]->owner->SendMessage("collision", message, m_boxColliders[k]->GetProperties());
					m_boxColliders[k]->owner->SendMessage("collision", message, m_boxColliders[i]->GetProperties());
				}
			}
		}
		return true;
	}

	bool PhysicsEngine::DoOverlap(Vector2 l1, Vector2 r1, Vector2 l2, Vector2 r2)
	{
		if (l1.x >= r2.x || l2.x >= r1.x) return false;
		if (l1.y >= r2.y || l2.y >= r1.y) return false;
		return true;
	}

	bool PhysicsEngine::RefreshEntities()
	{
		m_entityList.Clear();
		m_boxColliders.Clear();
		ArrayList<Entity*> entitiesInScene = owner->GetEntityList();
		for (int i = 0; i < entitiesInScene.Count(); i++)
		{
			BoxCollider* boxCollider = entitiesInScene[i]->GetComponentByType<BoxCollider>();
			if (boxCollider != 0)
			{
				m_boxColliders.Add(boxCollider);
			}
		}
		return false;
	}
}