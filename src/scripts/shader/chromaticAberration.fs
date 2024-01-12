precision mediump float;

uniform sampler2D tDiffuse;

varying vec2 vUv;

void main() {
  vec2 dir = normalize(vUv - 0.5);
  float power = distance(vUv, vec2(0.5));
  power = smoothstep(0.2, 0.7, power);

  vec3 color;
  color.r = texture2D(tDiffuse, vUv - dir * power * 0.00).r;
  color.g = texture2D(tDiffuse, vUv - dir * power * 0.02).g;
  color.b = texture2D(tDiffuse, vUv - dir * power * 0.04).b;

  gl_FragColor = vec4(color, 1.0);
}