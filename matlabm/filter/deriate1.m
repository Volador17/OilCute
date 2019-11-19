% 一阶差分
% input
% X:吸光度矩阵
% m:差分宽度
% output
% xc:处理后的光谱

function d=deriate1(X,m)
	[m1,n]=size(X);
	for i=1:m1-m
		d(i,:)=X(i+m,:)-X(i,:);
	end
	d((m1-m+1):m1,:)=zeros(m,n);
end
