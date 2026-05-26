export function BootstrapPage() {
  return (
    <main className="min-h-screen bg-slate-950 text-slate-100">
      <section className="mx-auto flex min-h-screen max-w-5xl flex-col justify-center px-6">
        <div className="rounded-2xl border border-white/10 bg-white/5 p-8 shadow-2xl backdrop-blur-xl">
          <p className="text-sm uppercase tracking-[0.2em] text-cyan-300">DeveloperStore</p>
          <h1 className="mt-4 text-4xl font-semibold">Frontend React inicializado</h1>
          <p className="mt-4 max-w-2xl text-slate-300">
            Base Vite, React, TypeScript, Tailwind CSS e TanStack Query pronta para evoluir as telas de Auth, Sales, Dashboard e Health.
          </p>
        </div>
      </section>
    </main>
  )
}
