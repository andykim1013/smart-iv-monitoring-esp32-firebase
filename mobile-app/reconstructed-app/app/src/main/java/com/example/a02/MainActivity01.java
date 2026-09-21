package com.example.a02;

import android.os.Bundle;

// [reconstructed-app 사본 전용 수정] androidx.activity.EdgeToEdge는 사용하지 않는 import였고,
// 빌드에 사용된 androidx.activity:activity:1.7.0에는 해당 클래스가 존재하지 않아 컴파일 에러가 발생함.
// 원문(recovered-source)에는 그대로 남아 있으며, 이 줄만 이 사본에서 제거함(docs/reconstruction-changes.md 참조).
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;


public class MainActivity01 extends AppCompatActivity {
private TextView textView03;
private TextView textViewend1;
private TextView textViewstart1;

private DatabaseReference Starttime;
private DatabaseReference Endtime; //데이터베이스 종료시간데이터 가져오기


@Override
protected void onCreate(Bundle savedInstanceState) {
super.onCreate(savedInstanceState);
setContentView(R.layout.activity_main01);
textViewend1 = findViewById(R.id.textViewend1);
textViewstart1 = findViewById(R.id.textViewstart1);
Endtime = FirebaseDatabase.getInstance().getReference().child("802").child("EndTime");
Starttime = FirebaseDatabase.getInstance().getReference().child("802").child("StartTime");

Button button = findViewById(R.id.button05); // 버튼ID에 맞게 설정
Endtime.addValueEventListener(new ValueEventListener() {
@Override
public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
if (dataSnapshot.exists()) {
// Firebase에서"time" 값 가져오기
String timeValue = dataSnapshot.getValue(String.class);
// TextView에 값 설정
textViewend1.setText(timeValue);
} else {
textViewend1.setText("상태: 공실");
}
}
@Override
public void onCancelled(@NonNull DatabaseError databaseError) {
// 오류 발생 시 처리
textViewend1.setText("Failed to load data: " + databaseError.getMessage());
}
});

Starttime.addValueEventListener(new ValueEventListener() {
@Override
public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
if (dataSnapshot.exists()) {
// Firebase에서"time" 값 가져오기
String timeValue = dataSnapshot.getValue(String.class);
// TextView에 값 설정
textViewstart1.setText(timeValue);
} else {
textViewstart1.setText("상태: 공실");
}
}
@Override
public void onCancelled(@NonNull DatabaseError databaseError) {
// 오류 발생 시 처리
textViewend1.setText("Failed to load data: " + databaseError.getMessage());
}
});

button.setOnClickListener(new View.OnClickListener() {
@Override
public void onClick(View v) {
// 새로운 액티비티로 이동하는 인텐트 생성
Intent intent = new Intent(MainActivity01.this, MainActivity.class);
startActivity(intent);
}
});
}
}