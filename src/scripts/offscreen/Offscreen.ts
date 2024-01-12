import * as THREE from 'three'
import { Canvas } from '../Canvas'

export class Offscreen {
  private renderTarget: THREE.WebGLRenderTarget
  private camera: THREE.OrthographicCamera
  private scene: THREE.Scene

  constructor(
    protected canvas: Canvas,
    private material: THREE.RawShaderMaterial,
    private dpr = 1,
  ) {
    this.renderTarget = new THREE.WebGLRenderTarget(canvas.size.width * dpr, canvas.size.height * dpr)
    this.camera = new THREE.OrthographicCamera()
    this.scene = new THREE.Scene()
    this.createScreen(material)
  }

  private createScreen(material: THREE.RawShaderMaterial) {
    const geometry = new THREE.PlaneGeometry(2, 2)
    const mesh = new THREE.Mesh(geometry, material)
    this.scene.add(mesh)
    return mesh
  }

  get texture() {
    return this.renderTarget.texture
  }

  get uniforms() {
    return this.material.uniforms
  }

  resize() {
    this.renderTarget.setSize(this.canvas.size.width * this.dpr, this.canvas.size.height * this.dpr)
  }

  render() {
    this.canvas.renderer.setRenderTarget(this.renderTarget)
    this.canvas.renderer.render(this.scene, this.camera)
  }
}
