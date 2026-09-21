package com.example.a02;

import android.os.Bundle;

import androidx.activity.EdgeToEdge;
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