package com.example.a02;

import android.os.Bundle;

// [reconstructed-app 사본 전용 수정] androidx.activity.EdgeToEdge는 사용하지 않는 import였고,
// 빌드에 사용된 androidx.activity:activity:1.7.0에는 해당 클래스가 존재하지 않아 컴파일 에러가 발생함.
// 원문(recovered-source)에는 그대로 남아 있으며, 이 줄만 이 사본에서 제거함(docs/reconstruction-changes.md 참조).
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import androidx.appcompat.app.AppCompatActivity;

public class MainActivity extends AppCompatActivity {

@Override
protected void onCreate(Bundle savedInstanceState) {
super.onCreate(savedInstanceState);
setContentView(R.layout.activity_main);

// 버튼을 찾고 클릭 리스너를 설정
Button button = findViewById(R.id.button2); // 버튼ID에 맞게 설정
button.setOnClickListener(new View.OnClickListener() {
@Override
public void onClick(View v) {
// 새로운 액티비티로 이동하는 인텐트 생성
Intent intent = new Intent(MainActivity.this, MainActivity01.class);
startActivity(intent);
}
});
}
}