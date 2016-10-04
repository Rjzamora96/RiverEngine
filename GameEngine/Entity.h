#pragma once

class Component;

class Entity
{
	enum
	{
		MAX_COMPONENTS = 10,
		MAX_NAME_LEN = 30
	};
public:
	Entity();
	~Entity();
	void SetName(const char* const name);
	const char* GetName() const { return m_name; }
	bool AddComponent(Component* c, const char* const name);
	template <class T> T* GetComponentByType() const;
	bool Update(float dt);
protected:
	bool Initialize();
private:
	char m_name[MAX_NAME_LEN];
	Component* m_components[MAX_COMPONENTS];
};

template<class T>
inline T * Entity::GetComponentByType() const
{
	for (int j = 0; j < MAX_COMPONENTS; ++j)
	{
		if (!m_components[j]) continue;
		if (typeid(T) == typeid(*m_components[j]))
		{
			if (m_components[j]->IsDisabled()) return 0;
			return static_cast<T*>(m_components[j]);
		}
	}
	return 0;
}
