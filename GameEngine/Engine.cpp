#include "Engine.h"

#include "Vector2.h"
#include "Component.h"
#include "Transform.h"
#include "Sprite.h"
#include "Input.h"

#include "LuaBridge.h"
#include "RefCountedPtr.h"
#include "Scene.h"

extern "C" {
# include "lua.h"
# include "lauxlib.h"
# include "lualib.h"
}

using namespace luabridge;

namespace RiverEngine
{

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
			.addStaticFunction("IsKeyDown", &Input::IsKeyDown)
			.addStaticFunction("IsKeyPressed", &Input::IsKeyPressed)
			.addStaticFunction("IsKeyReleased", &Input::IsKeyReleased)
			.endClass();
		getGlobalNamespace(L)
			.beginClass<Component>("Component")
			.addConstructor<void(*)(void)>()
			.addStaticFunction("Property", &Component::Property)
			.addData("entity", &Component::owner)
			.endClass()
			.deriveClass<Transform, Component>("Transform")
			.addData("rotation", &Transform::rotation)
			.addData("scale", &Transform::scale)
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
			.addFunction("sendMessage", &Entity::SendMessage)
			.endClass();
		Component::AssignState(L);
		Entity::AssignState(L);
		Input::InitializeBindings();
		Entity* e = new Entity();
		e->LoadComponents("Steve.entity");
		Sprite* sprite = new Sprite("cat.png");
		e->AddComponent(sprite, "Sprite");
		Sprite::addTexture("dog.png");
		Sprite::addSprite(sprite, "cat.png");
		//Component* moveLeft = new Component();
		//e->AddComponent(moveLeft, "MoveLeft");
		//moveLeft->SetScript("moveLeft.lua");
		//moveLeft->Init();
		Entity* c = new Entity();
		c->AddComponent(new Transform(), "Transform");
		Sprite* sprite2 = new Sprite("camera.png");
		c->AddComponent(sprite2, "Sprite");
		Sprite::addSprite(sprite2, "camera.png");
		Component* cameraMove = new Component();
		c->AddComponent(cameraMove, "CameraMove");
		c->AddTag("Camera");
		cameraMove->SetScript("cameraMove.lua");
		cameraMove->Init();
		Scene::SetActiveScene(new Scene());
		Scene::AddEntity(e);
		Scene::AddEntity(c);
		return true;
	}

}