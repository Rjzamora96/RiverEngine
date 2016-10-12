#include "Engine.h"

#include "Vector2.h"
#include "Component.h"
#include "Transform.h"
#include "Sprite.h"
#include "Input.h"

#include "LuaBridge.h"
#include "RefCountedPtr.h"
extern "C" {
# include "lua.h"
# include "lauxlib.h"
# include "lualib.h"
}

using namespace luabridge;

namespace RiverEngine
{
	Entity* Engine::testEntity;

	Engine::Engine()
	{
	}


	Engine::~Engine()
	{
	}
	using namespace RiverEngine;
	bool Engine::Init()
	{
		luabridge::lua_State* L = luaL_newstate();
		luaL_openlibs(L);
		getGlobalNamespace(L)
			.beginClass<Vector2>("Vector2")
			.addConstructor<void(*)(void)>()
			.addData("x", &Vector2::x)
			.addData("y", &Vector2::y)
			.endClass();
		getGlobalNamespace(L)
			.beginClass<Input>("Input")
			.addStaticFunction("isKeyDown", &Input::IsKeyDown)
			.endClass();
		getGlobalNamespace(L)
			.beginClass<Component>("Component")
			.addConstructor<void(*)(void)>()
			.addStaticFunction("property", &Component::Property)
			.addData("entity", &Component::owner)
			.endClass()
			.deriveClass<Transform, Component>("Transform")
			.addData("rotation", &Transform::rotation)
			.addData("position", &Transform::position)
			.endClass();
		getGlobalNamespace(L)
			.deriveClass<Sprite, Component>("Sprite")
			.addProperty("image", &Sprite::GetSprite, &Sprite::SetSprite)
			.endClass();
		getGlobalNamespace(L)
			.beginClass<Entity>("Entity")
			.addConstructor<void(*)(void)>()
			.addData("transform", &Entity::transform)
			.addData("sprite", &Entity::sprite)
			.addFunction("getComponent", &Entity::GetComponent)
			.endClass();
		Component::AssignState(L);
		Entity::AssignState(L);
		Input::InitializeBindings();
		testEntity = new Entity();
		Sprite* sprite = new Sprite("cat.png");
		Sprite::addTexture("dog.png");
		Sprite::addSprite(sprite, "cat.png");
		testEntity->AddComponent(sprite, "Sprite");
		Component* moveLeft = new Component();
		testEntity->AddComponent(moveLeft, "MoveLeft");
		moveLeft->SetScript("moveLeft.lua");
		moveLeft->Init();
		return true;
	}

}