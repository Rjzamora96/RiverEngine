#include "Entity.h"

#include <string.h>
#include "Component.h"

Entity::Entity()
{
	memset(m_components, 0, MAX_COMPONENTS * sizeof(m_components[0]));
}


Entity::~Entity()
{
}

void Entity::SetName(const char * const name)
{
	for (int j = 0; j < MAX_NAME_LEN; ++j)
	{
		m_name[j] = name[j];
		if (!name[j]) return;
	}
	m_name[MAX_NAME_LEN - 1] = 0;
}

bool Entity::AddComponent(Component * c, const char * const name)
{
	for (int j = 0; j < MAX_COMPONENTS; ++j)
	{
		if (!m_components[j])
		{
			m_components[j] = c;
			c->SetOwner(this);
			c->SetName(name);
			break;
		}
	}
	return true;
}

bool Entity::Update(float dt)
{
	for (int j = 0; j < MAX_COMPONENTS; ++j)
	{
		if (m_components[j] && m_components[j]->IsEnabled())
		{
			bool result = m_components[j]->Update(dt);
		}
	}
	return true;
}

bool Entity::Initialize()
{
	for (int j = 0; j < MAX_COMPONENTS; ++j)
	{
		if (m_components[j])
		{
			if (!m_components[j]->Init()) return false;
		}
	}
	return true;
}