precision mediump float;

uniform sampler2D tDiffuse;
uniform vec2 uResolution;

varying vec2 vUv;

const float SCALE = 0.1;

float sdSegment(in vec2 p, in vec2 a, in vec2 b) {
  vec2 pa = p - a, ba = b - a;
  float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
  return length(pa - ba * h);
}

float sdCircle(vec2 p, float r) {
  return length(p) - r;
}

float sdBox(in vec2 p, in vec2 b) {
  vec2 d = abs(p) - b;
  return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

float hMeter(vec2 uv, float dir) {
  float l1 = sdSegment(uv, vec2(dir * 0.7, 0.0) * SCALE, vec2(dir * 1.7, 0.0) * SCALE);
  float l2 = sdSegment(uv, vec2(dir * (0.7 + 1.0 / 4.0 * 0.0), -0.08) * SCALE, vec2(dir * (0.7 + 1.0 / 4.0 * 0.0), 0.08) * SCALE);
  float l3 = sdSegment(uv, vec2(dir * (0.7 + 1.0 / 4.0 * 2.0), -0.08) * SCALE, vec2(dir * (0.7 + 1.0 / 4.0 * 2.0), 0.08) * SCALE);
  float l4 = sdSegment(uv, vec2(dir * (0.7 + 1.0 / 4.0 * 3.0), -0.08) * SCALE, vec2(dir * (0.7 + 1.0 / 4.0 * 3.0), 0.08) * SCALE);
  float l5 = sdSegment(uv, vec2(dir * (0.7 + 1.0 / 4.0 * 4.0), -0.08) * SCALE, vec2(dir * (0.7 + 1.0 / 4.0 * 4.0), 0.08) * SCALE);
  return min(l5, min(l4, min(l3, min(l1, l2))));
}

float vMeter(vec2 uv, float dir) {
  float span = 1.0 / 6.0;
  float l1 = sdSegment(uv, vec2(-0.08, dir * (0.2 + span * 0.0)) * SCALE, vec2(0.08, dir * (0.2 + span * 0.0)) * SCALE);
  float l2 = sdSegment(uv, vec2(-0.08, dir * (0.2 + span * 1.0)) * SCALE, vec2(0.08, dir * (0.2 + span * 1.0)) * SCALE);
  float l3 = sdSegment(uv, vec2(-0.08, dir * (0.2 + span * 2.0)) * SCALE, vec2(0.08, dir * (0.2 + span * 2.0)) * SCALE);
  float l4 = sdSegment(uv, vec2(-0.08, dir * (0.2 + span * 3.0)) * SCALE, vec2(0.08, dir * (0.2 + span * 3.0)) * SCALE);
  float l5 = sdSegment(uv, vec2(-0.08, dir * (0.2 + span * 4.0)) * SCALE, vec2(0.08, dir * (0.2 + span * 4.0)) * SCALE);
  float l6 = sdSegment(uv, vec2(-0.08, dir * (0.2 + span * 5.0)) * SCALE, vec2(0.08, dir * (0.2 + span * 5.0)) * SCALE);
  float l7 = sdSegment(uv, vec2(-0.08, dir * (0.2 + span * 6.0)) * SCALE, vec2(0.08, dir * (0.2 + span * 6.0)) * SCALE);
  float l8 = sdSegment(uv, vec2(-0.08, dir * (0.2 + span * 7.0)) * SCALE, vec2(0.08, dir * (0.2 + span * 7.0)) * SCALE);
  float v = min(l1, min(l2, min(l3, min(l4, min(l5, min(l6, min(l7, l8)))))));
  v = 1.0 - smoothstep(0.0, 0.002, v);
  return v;
}

void main() {
  vec2 aspect = vec2(uResolution.x / uResolution.y, 1.0);
  vec2 uv = vUv * 2.0 - 1.0;
  uv *= aspect;

  float c1 = abs(sdCircle(uv, 0.5 * SCALE));
  float b1 = sdBox(uv, vec2(0.15, 1.0) * SCALE);
  float b2 = sdBox(uv, vec2(1.0, 0.15) * SCALE);
  c1 = max(c1, -b1);
  c1 = max(c1, -b2);
  
  float c2 = abs(sdCircle(uv, 1.0 * SCALE));
  float b3 = sdBox(uv, vec2(0.9, 10.0) * SCALE);
  c2 = max(c2, -b3);

  float c3 = abs(sdCircle(uv, 0.01 * SCALE));

  float hm1 = hMeter(uv, -1.0);
  float hm2 = hMeter(uv, 1.0);

  float glow;
  glow = min(c1, c2);
  glow = min(glow, c3);
  glow = min(glow, hm1);
  glow = min(glow, hm2);

  glow = 1.0 - glow;
  glow = 0.01 / abs(1.0 - pow(glow, 5.0));
  
  float vm1 = vMeter(uv, -1.0);
  float vm2 = vMeter(uv, 1.0);
  vec3 scopeColor = vec3((vm1 + vm2) * 0.5);
  scopeColor += vec3(0.89, 0.34, 0.11) * glow;

  float hLine = sdSegment(uv, vec2(-1.0, 0.0) * aspect, vec2(1.0, 0.0) * aspect);
  float vLine = sdSegment(uv, vec2(0.0, -1.0), vec2(0.0, 1.0));
  hLine = smoothstep(0.0, 0.002 + pow(abs(uv.x), 2.0) * 0.001, hLine);
  vLine = smoothstep(0.0, 0.002 + pow(abs(uv.y * aspect.x), 2.0) * 0.001, vLine);
  float line = hLine * vLine;

  vec4 diffuse = texture2D(tDiffuse, vUv);

  vec3 color = diffuse.rgb;
  color = mix(color * vec3(0.5), color, line);
  color += scopeColor;


  gl_FragColor = vec4(color, 1.0);
}