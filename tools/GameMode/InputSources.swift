import Foundation
import Carbon

// Injectable operations keep tests from changing the user's real input source.
struct InputSourceService {
    let currentID: () throws -> String
    let englishID: () throws -> String
    let select: (String) throws -> Void

    func prepare() throws -> InputSwitch {
        try InputSwitch(originalID: currentID(), englishID: englishID())
    }

    func ensureEnglish(_ id: String) throws {
        if try currentID() != id { try select(id) }
    }

    func restore(_ change: InputSwitch) throws {
        // Respect a source the user chose manually after entering game mode.
        if try currentID() == change.englishID && change.originalID != change.englishID {
            try select(change.originalID)
        }
    }

    static let system = InputSourceService(
        currentID: { try onInputMain {
            guard let source = TISCopyCurrentKeyboardInputSource()?.takeRetainedValue(),
                  let id = inputProperty(source, kTISPropertyInputSourceID) as? String else {
                throw inputError("无法读取当前输入法。")
            }
            return id
        } },
        englishID: { try onInputMain {
            let sources = inputSources().map { source in
                KeyboardSource(id: inputProperty(source, kTISPropertyInputSourceID) as? String ?? "",
                               languages: inputProperty(source, kTISPropertyInputSourceLanguages) as? [String] ?? [],
                               isKeyboardLayout: (inputProperty(source, kTISPropertyInputSourceType) as? String) == (kTISTypeKeyboardLayout as String),
                               selectable: (inputProperty(source, kTISPropertyInputSourceIsSelectCapable) as? Bool) == true)
            }
            guard let id = englishSourceID(sources) else {
                throw inputError("没有可用的英文键盘。请在系统设置 → 键盘 → 输入法中添加 ABC 或美国键盘，或取消自动切换。")
            }
            return id
        } },
        select: { id in try onInputMain {
            guard let source = inputSources().first(where: { (inputProperty($0, kTISPropertyInputSourceID) as? String) == id }) else {
                throw inputError("输入法已不可用：\(id)")
            }
            let result = TISSelectInputSource(source)
            guard result == noErr else { throw inputError("输入法切换失败（\(result)）。") }
            // The system can refuse a change despite a successful API status.
            guard let selected = TISCopyCurrentKeyboardInputSource()?.takeRetainedValue(),
                  (inputProperty(selected, kTISPropertyInputSourceID) as? String) == id else {
                throw inputError("系统没有完成输入法切换，请手动选择英文键盘。")
            }
        } })
}

private func inputError(_ message: String) -> NSError {
    NSError(domain: "GameMode.InputSource", code: 1, userInfo: [NSLocalizedDescriptionKey: message])
}

private func onInputMain<T>(_ body: () throws -> T) rethrows -> T {
    if Thread.isMainThread { return try body() }
    return try DispatchQueue.main.sync(execute: body)
}

private func inputSources() -> [TISInputSource] {
    TISCreateInputSourceList(nil, false)?.takeRetainedValue() as? [TISInputSource] ?? []
}

private func inputProperty(_ source: TISInputSource, _ key: CFString) -> AnyObject? {
    guard let value = TISGetInputSourceProperty(source, key) else { return nil }
    return Unmanaged<AnyObject>.fromOpaque(value).takeUnretainedValue()
}
