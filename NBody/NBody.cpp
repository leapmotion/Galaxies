#include <iostream>
#include <Eigen/Dense>

using Eigen::Vector4f;
using Eigen::Quaternionf;

float gravConstant;
float combineDistance;

struct BlackHole {
  Vector4f position;
  Vector4f velocity;
  float mass;
  int id;
  Quaternionf rotation;
};

struct Universe {
  BlackHole* _blackHoles;
  int _numBlackHoles;

  float _time;
  int _frames;

  Universe(int numBlackHoles) {
    _numBlackHoles = numBlackHoles;
    _blackHoles = new BlackHole[numBlackHoles];
    _time = 0;
    _frames = 0;
  }

  ~Universe() {
    delete[] _blackHoles;
  }

  void CopyFrom(Universe* other) {
    _numBlackHoles = other->_numBlackHoles;
    for (int i = 0; i < other->_numBlackHoles; i++) {
      _blackHoles[i] = other->_blackHoles[i];
    }

    _time = other->_time;
    _frames = other->_frames;
  }

  void Step() {
    const float DELTA_TIME = 1.0f / 60.0f;
    const float TIMESTEP_FACTOR = 1.0f;
    const int SUB_FRAMES = 10;
    const float PLANET_DT = 1.0f / SUB_FRAMES;
    const float COMBINED_DT = PLANET_DT * TIMESTEP_FACTOR;

    _time += DELTA_TIME;
    _frames += 1;

    float preStepConstant = gravConstant * PLANET_DT * TIMESTEP_FACTOR;
    float combineDistSqrd = combineDistance * combineDistance;

    for (int step = 0; step < SUB_FRAMES; step++) {
      //Force accumulation
      {
        BlackHole* srcA = _blackHoles;
        for (int indexA = 0; indexA < _numBlackHoles; indexA++, srcA++) {

          BlackHole* srcB = _blackHoles + indexA + 1;
          for (int indexB = indexA + 1; indexB < _numBlackHoles; indexB++, srcB++) {
            Vector4f toB = srcB->position - srcA->position;

            float dist = toB.norm();
            float forceConst = srcA->mass * srcB->mass * preStepConstant / (dist * dist * dist);

            Vector4f force = toB * forceConst;

            srcA->velocity += force;
            srcB->velocity -= force;
          }
        }
      }

      //Position integration
      {
        BlackHole* src = _blackHoles;
        for (int i = 0; i < _numBlackHoles; i++, src++) {
          src->position += src->velocity * COMBINED_DT;
        }
      }

      //Black hole combination
      {
        BlackHole* srcA = _blackHoles;
        for (int indexA = 0; indexA < _numBlackHoles; indexA++, srcA++) {

          BlackHole* srcB = _blackHoles + indexA + 1;
          for (int indexB = indexA + 1; indexB < _numBlackHoles; indexB++, srcB++) {
            Vector4f delta = srcA->position - srcB->position;
            float distSqrd = delta[0] * delta[0] + delta[1] * delta[1] + delta[2] * delta[2];

            if (distSqrd <= combineDistSqrd) {
              float inverseMass = 1.0f / (srcA->mass + srcB->mass);
              srcA->position = (srcA->position * srcA->mass + srcB->position * srcB->mass) * inverseMass;
              srcA->velocity = (srcA->velocity * srcA->mass + srcB->velocity * srcB->mass) * inverseMass;
              srcA->mass += srcB->mass;
              srcA->id = srcA->id | srcB->id;

              _numBlackHoles--;
              *srcB = *(_blackHoles + _numBlackHoles);

              indexB--;
              srcB--;
            }
          }
        }
      }
    }
  }
};

extern "C" __declspec(dllexport) void __stdcall SetParams(float gravity, float combineDist) {
  gravConstant = gravity;
  combineDistance = combineDist;
}

extern "C" __declspec(dllexport) Universe* __stdcall CreateGalaxy(int numBodies) {
  return new Universe(numBodies);
}

extern "C" __declspec(dllexport) void __stdcall DestroyGalaxy(Universe* ptr) {
  delete ptr;
}

extern "C" __declspec(dllexport) void __stdcall CopyGalaxy(Universe* from, Universe* to) {
  to->CopyFrom(from);
}

extern "C" __declspec(dllexport) void __stdcall StepGalaxy(Universe* ptr) {
  ptr->Step();
}
