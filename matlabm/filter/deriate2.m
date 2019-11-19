% 二阶差分
% input
% X:吸光度矩阵
% m:差分宽度
% output
% xc:处理后的光谱
function d=deriate2(X,m)
	[m1,n]=size(X);
	for i=1:m1-m-2
		d(i,:)=X(i+2+m,:)-2*X(i+1+m,:)+X(i,:);
	end
	d((m1-m-1):m1,:)=zeros(m+2,n);
end