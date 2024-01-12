precision mediump float;

struct Image {
  sampler2D source;
  vec2 coveredScale;
};

uniform Image uCurrent;
uniform Image uNext;
uniform vec2 uMouse;
uniform float uZoom;
uniform float uProgress;

varying vec2 vUv;

const float sensitivity = 2.0; // 2.0: 画像端部まで照準が合う

vec3 getColor(Image image) {
  vec2 uv = (vUv - 0.5) * image.coveredScale * uZoom + 0.5;
  // mouseで移動する（倍率も考慮）
  uv += uMouse * (1.0 - uZoom) * 0.25 * sensitivity;
  // 倍率を変えたときに場所を固定する
  uv += uMouse * uZoom * 0.25 * sensitivity;

  vec2 dir = normalize(vUv - 0.5);
  float power = distance(vUv, vec2(0.5));
  power = smoothstep(0.2, 0.7, power);

  vec3 color = texture2D(image.source, uv - power * dir * 0.005).rgb;
  return color;
}

void main() {
  vec3 current = getColor(uCurrent);
  vec3 next = getColor(uNext);
  float progress = uProgress * 1.2;
  vec3 color = mix(next, current, smoothstep(progress - 0.2, progress, 1.0 - (vUv.x * vUv.y)));
  color *= 0.7;
  gl_FragColor = vec4(color, 1.0);
}