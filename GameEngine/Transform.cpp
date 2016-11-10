#include "Transform.h"
#include <math.h>

bool RiverEngine::Transform::Update(float dt)
{
	/*
	Point parentPosition = new Point(0, 0);
	if (Owner.EParent != null) parentPosition = Owner.EParent.Preview.Position;
	double scale = (Owner.EParent != null) ? Owner.EParent.Preview.Scale : 1.0;
	Point unrotatedPoint = new Point((_localPosition.X * scale) + parentPosition.X, (_localPosition.Y * scale) + parentPosition.Y);
	double cs = Math.Cos((Math.PI * Rotation) / 180);
	double sn = Math.Sin((Math.PI * Rotation) / 180);
	return new Point((unrotatedPoint.X * cs) - (unrotatedPoint.Y * sn), (unrotatedPoint.X * sn) + (unrotatedPoint.Y * cs));
	*/
	position->x = localPosition->x;
	position->y = localPosition->y;
	rotation = localRotation;
	scale = localScale;
	Entity* parent = owner->parent;
	if (parent != 0)
	{
		Transform* parentTransform = parent->GetComponentByType<Transform>();
		if (parentTransform != 0)
		{
			scale = localScale * parentTransform->scale;
			rotation = localRotation + parentTransform->rotation;
			Vector2 unrotated((localPosition->x * parentTransform->scale), (localPosition->y * parentTransform->scale));
			float cs = cosf(rotation);
			float sn = sinf(rotation);
			position->x = (unrotated.x * cs) - (unrotated.y * sn) + parentTransform->position->x;
			position->y = (unrotated.x * sn) + (unrotated.y * cs) + parentTransform->position->y;
		}
	}
	return true;
}
