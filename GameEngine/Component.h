#pragma once

#include "Entity.h"

class Component
{
	enum
	{
		MAX_COMPONENTS = 10,
		MAX_NAME_LEN = 30
	};
public:
	Component();
	~Component();
	void SetName(const char* const name);
	void SetOwner(Entity* owner) { m_owner = owner; }
	bool Init();
	virtual bool Update(float dt) { dt; return true; }
	virtual bool Draw() { return true; }
	virtual bool Initialize() { return true; }
	void Enable(bool enabled = true) { m_enabled = enabled; }
	bool IsEnabled() const { return m_enabled; }
	bool IsDisabled() const { return !m_enabled; }
	static void SetBreak(bool enabled = true);
	static void ToggleBreak() { s_breakable = !s_breakable; }
	static void Break(bool condition = true, bool keepBreakable = true);
	static void BreakIf(bool condition = true);
protected:
	template <class T> T* GetSiblingComponent() { return m_owner->GetComponentByType<T>(); }
	char m_name[MAX_NAME_LEN];
	bool m_enabled = true;
	Entity* m_owner;
private:
	static bool s_breakable;

};

