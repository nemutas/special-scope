import * as THREE from 'three'
import { Canvas } from '../Canvas'
import { Offscreen } from './Offscreen'
import vertexShader from '../shader/screen.vs'
import fragmentShader from '../shader/vignette.fs'

export class Vignette extends Offscreen {
  constructor(canvas: Canvas, source: THREE.Texture) {
    const material = new THREE.RawShaderMaterial({
      uniforms: {
        tDiffuse: { value: source },
        uResolution: { value: [canvas.size.width, canvas.size.height] },
      },
      vertexShader,
      fragmentShader,
    })

    super(canvas, material)
  }

  resize() {
    super.resize()
    this.uniforms.uResolution.value = [this.canvas.size.width, this.canvas.size.height]
  }
}
